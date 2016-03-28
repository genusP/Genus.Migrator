using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Model.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations
{
    public abstract class Migration
    {
        private readonly Lazy<IEnumerable<MigrationOperation>> _upOperaions;
        private readonly Lazy<IEnumerable<MigrationOperation>> _downOperaions;

        public Migration()
        {
            _upOperaions = new Lazy<IEnumerable<MigrationOperation>>(() =>
            {
                var builder = new MigrationBuilder();
                Up(builder);
                return builder.Operations;
            });

            _downOperaions = new Lazy<IEnumerable<MigrationOperation>>(() =>
            {
                var builder = new MigrationBuilder();
                Down(builder);
                return builder.Operations;
            });
        }

        protected abstract void Up(MigrationBuilder migration);

        protected abstract void Down(MigrationBuilder migration);

        public abstract void BuildTargetModel(ModelBuilder modelBuilder);

        public IEnumerable<MigrationOperation> UpOperations
            => _upOperaions.Value;

        public IEnumerable<MigrationOperation> DownOperations
            => _downOperaions.Value;
    }
}
