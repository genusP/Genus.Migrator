using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Genus.Migrator.DependecyInjection;
using Microsoft.Extensions.Options;
using Genus.Migrator.Model.Builder;

namespace Genus.Migrator.Migrations.Design
{
    public class MigrationCommands
    {

        public MigrationCommands(IOrmAdapter adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));
            if (string.IsNullOrWhiteSpace(adapter.Environment))
                adapter.Environment = "Development";
            OrmAdapter = adapter;
            ServiceProvider = adapter.ServiceProvider;
        }

        protected IOrmAdapter OrmAdapter { get; }

        public IServiceProvider ServiceProvider
        {
            get;
        }

        public void AddMigration(
            string targetAssemblyName,
            IModel currentModel,
            IModel targetModel, 
            string outputDir,
            bool skipEmptyMigration
            )
        {
            if (string.IsNullOrEmpty(targetAssemblyName))
                throw new ArgumentException("Required not empty value", nameof(targetAssemblyName));
            if (targetModel == null)
                throw new ArgumentNullException(nameof(targetModel));
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Need value", nameof(outputDir));

            var assembly = Assembly.Load(new AssemblyName(targetAssemblyName));

            var comparer = ServiceProvider.GetRequiredService<IModelComparer>();
            var upOperations = comparer.CompareModel(currentModel, targetModel);
            if (!upOperations.Any())
                return;
            var downOperations = upOperations.Any()
                ? comparer.CompareModel(targetModel, currentModel)
                : Enumerable.Empty<MigrationOperation>();

            var factory = ServiceProvider.GetRequiredService<ICodeGeneratorFactory>();
            var ns = GetNamespace(targetAssemblyName);
            var id = GetMigrationId(targetAssemblyName);
            var className = "_" + id;
            var codeGenerator = factory.MigrationCodeGenerator(ns, className);
            var migrationStringBuilder = new IndentedStringBuilder();
            codeGenerator.GenerateMigration(upOperations, downOperations, migrationStringBuilder);

            var targetModelStringBuilder = new IndentedStringBuilder();
            codeGenerator.GenerateTargetModel(targetModel, targetModelStringBuilder);

            var snapshotName = GetSnapshotName(targetAssemblyName);
            var snapshotGenerator = factory.SnapshotGenerator(ns, snapshotName);
            var snapshotBuilder = new IndentedStringBuilder();
            snapshotGenerator.Generate(targetModel, snapshotBuilder);

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            File.WriteAllText(Path.Combine(outputDir, id+codeGenerator.Extension), migrationStringBuilder.ToString());
            File.WriteAllText(Path.Combine(outputDir, $"{id}.Designer{codeGenerator.Extension}"), targetModelStringBuilder.ToString());
            File.WriteAllText(Path.Combine(outputDir, snapshotName+ snapshotGenerator.Extension), snapshotBuilder.ToString());

        }

        protected virtual string GetNamespace(string targetAssemblyName)
        {
            return targetAssemblyName + ".Migrations";
        }

        protected virtual string GetMigrationId(string targetAssemblyName)
        {
            return $"{DateTime.Now:yyMMddHHmmssff}";
        }

        protected virtual string GetSnapshotName(string targetAssemblyName)
        {
            var name = targetAssemblyName.Split('.').Last();
            return name+"Snapshot";
        }

        public void RemoveMigration()
        {
            var migrationsSource = ServiceProvider.GetRequiredService<IMigrationsSource>();
            var migrationLog = ServiceProvider.GetRequiredService<IMigrationLog>();

            var migrations = migrationsSource.GetMigrations(OrmAdapter.StartupAssembly);
            if (migrations.Any())
            {
                var migrationInfo = migrations.Last.Value;
                var targetMigrationInfo = migrations.Last.Previous.Value;
                var migration = migrationsSource.CreateMigration(migrationInfo.Value);

                if (migrationLog.AppliedMigrations.Contains(migrationInfo.Key))
                    throw new InvalidOperationException($"Unapply migration: {migrationInfo.Key}");

                Migration targetMigration = null;

                if (migrations.Last.Previous != null)
                    targetMigration = migrationsSource.CreateMigration(targetMigrationInfo.Value);

                var targetAssemblyName = migrationInfo.Value.Assembly.GetName().FullName;

                var migrationCodeGeneratorFactory = ServiceProvider.GetRequiredService<ICodeGeneratorFactory>();
                var migrationCodeGenerator = migrationCodeGeneratorFactory.MigrationCodeGenerator("a", "b");
                var migrationsDir = GetMigrationsDir( targetMigrationInfo.Value);
                var migrationFileWithOutExt = Path.Combine(migrationsDir, migrationInfo.Key);
                var migrationFile = migrationFileWithOutExt + migrationCodeGenerator.Extension;
                var metadataFile = migrationFileWithOutExt + ".Designer" + migrationCodeGenerator.Extension;
                var modelSnapshotName = GetSnapshotName(targetAssemblyName);
                var modelSnapshotFile = Path.Combine(migrationsDir, modelSnapshotName + migrationCodeGenerator.Extension);

                if (File.Exists(migrationFile))
                    File.Delete(migrationFile);
                if (File.Exists(metadataFile))
                    File.Delete(metadataFile);

                if (targetMigration == null && File.Exists(modelSnapshotFile))
                    File.Delete(modelSnapshotFile);
                else
                {
                    var snapshotGenerator = migrationCodeGeneratorFactory.SnapshotGenerator(GetNamespace(targetAssemblyName), modelSnapshotName);
                    var targetModelBuilder = new ModelBuilder();
                    targetMigration.BuildTargetModel(targetModelBuilder);
                    var snapshotBuilder = new IndentedStringBuilder();
                    snapshotGenerator.Generate(targetModelBuilder.Build(), snapshotBuilder);

                    File.WriteAllText(modelSnapshotFile, snapshotBuilder.ToString());
                }
            }
        }

        protected virtual string GetMigrationsDir(TypeInfo migrationTypeInfo)
            => "Migrations";

        public virtual string ApplyMigration(string migrationName, ProviderName providerName, bool onlyScript=false)
        {
            if (string.IsNullOrWhiteSpace(migrationName))
                throw new ArgumentException("Need migration name.");
            
            var currentMigrationName = ServiceProvider.GetRequiredService<IMigrationLog>().AppliedMigrations.LastOrDefault();
            bool isDownMigration;
            var migrations = GetMigartionsForApply(currentMigrationName, migrationName, out isDownMigration);
            var sqlGenerator = GetSqlGenerator(providerName);
            var sqlScripts = GetSqlScriptsByMigration(sqlGenerator, isDownMigration, migrations.Reverse());
            var sb = new StringBuilder();
            foreach (var item in sqlScripts)
            {
                sb.Append("--")
                    .AppendLine(item.Key)
                    .AppendLine(sqlGenerator.BatchTerminator);
                foreach (var script in item.Value)
                {
                    sb.AppendLine(script);
                    sb.AppendLine(sqlGenerator.BatchTerminator);
                    sb.AppendLine();
                    if (onlyScript == false)
                        OrmAdapter.ExecuteSql(script);
                }
            }
            return sb.ToString();
        }

        protected virtual SqlGenerator GetSqlGenerator(ProviderName providerName)
        {
            var sqlGenerators = ServiceProvider.GetRequiredService<IOptions<MigratorOptions>>().Value.SqlGenerators;
            Lazy<SqlGenerator> geneartorFactory;
            if (sqlGenerators.TryGetValue(providerName, out geneartorFactory))
                return geneartorFactory.Value;
            return null;
        }

        protected virtual IEnumerable<KeyValuePair<string, TypeInfo>> GetMigartionsForApply(
            string currentMigrationName, 
            string migrationName,
            out bool down)
        {
            Dictionary<string, TypeInfo> applied = new Dictionary<string, TypeInfo>();
            Dictionary<string, TypeInfo> notApplied = new Dictionary<string, TypeInfo>();

            var a = false;
            var source = ServiceProvider.GetRequiredService<IMigrationsSource>();
            foreach (var migrationInfo in source.GetMigrations(null).OrderByDescending(_=>_.Key))
            {
                if (migrationInfo.Key.Substring(1) == currentMigrationName)
                    a = true;
                if (a == true)
                {
                    applied.Add(migrationInfo.Key, migrationInfo.Value);
                }
                else {
                    notApplied.Add(migrationInfo.Key, migrationInfo.Value);
                }
            }
            if (applied.ContainsKey(migrationName)
                || migrationName.Equals("0", StringComparison.OrdinalIgnoreCase) == true)
            {
                down = true;
                return applied.Reverse().TakeWhile(m => m.Key != migrationName);
            }
            if (notApplied.ContainsKey(migrationName)
                || migrationName.Equals("last", StringComparison.OrdinalIgnoreCase) == true)
            {
                down = false;
                return notApplied.TakeWhile(m => m.Key != migrationName);
            }
            throw new InvalidOperationException($"Migration {migrationName} not found");
        }

        protected virtual IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetSqlScriptsByMigration(
            SqlGenerator sqlGenerator, 
            bool isDownMigration,
            IEnumerable<KeyValuePair<string, TypeInfo>> migrations)
        {
            var migrationLog = ServiceProvider.GetRequiredService<IMigrationLog>();
            if (!migrationLog.IsExist())
                yield return new KeyValuePair<string, IEnumerable<string>>("Migrator log table", new[] { migrationLog.GetCreateSqlScript() });
            foreach (var migration in migrations)
            {
                var migrationId = migration.Key.Substring(1);
                var migrationInst = ServiceProvider.GetRequiredService<IMigrationsSource>().CreateMigration(migration.Value);
                var operations = isDownMigration
                                    ? migrationInst.DownOperations
                                    : migrationInst.UpOperations;
                yield return new KeyValuePair<string, IEnumerable<string>>(migrationId, sqlGenerator.Generate(operations));
                if (isDownMigration)
                    yield return new KeyValuePair<string, IEnumerable<string>>(
                                            "Delete migation from log", 
                                            new[] { migrationLog.GetDeleteScript(migrationId) });
                else
                    yield return new KeyValuePair<string, IEnumerable<string>>(
                                            "Insert migation to log",
                                            new[] { migrationLog.GetInsertScript(migrationId) });
            }
        }
    }
}
