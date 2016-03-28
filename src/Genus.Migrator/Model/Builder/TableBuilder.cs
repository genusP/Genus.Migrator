using Genus.Migrator.Model.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class TableBuilder:DbObjectBuider<TableBuilder>
    {
        private readonly Lazy<SortedDictionary<string,FieldBuilder>> _fields 
            = new Lazy<SortedDictionary<string, FieldBuilder>>();
        private readonly Lazy<SortedDictionary<string,IndexBuilder>> _indexes 
            = new Lazy<SortedDictionary<string, IndexBuilder>>();
        private readonly Lazy<SortedDictionary<Tuple<string,string,string>, AssociationBuilder>> _associations 
            = new Lazy<SortedDictionary<Tuple<string, string, string>, AssociationBuilder>>();

        public TableBuilder()
        {
        }

        protected PrimaryKeyBuilder PrimaryKey { get; set; }

        protected Dictionary<ProviderName, string> WithDict { get; } = new Dictionary<ProviderName, string>();
        
        public PrimaryKeyBuilder HasKey(params string[] fields)
        {

            PrimaryKey = new PrimaryKeyBuilder(this, fields);
            return PrimaryKey;
        }

        public PrimaryKeyBuilder HasKey(params FieldBuilder[] fb)
        {
            PrimaryKey = new PrimaryKeyBuilder(fb);
            return PrimaryKey;
        }

        public TableBuilder Fields(params Action<TableBuilder>[] setupActions)
        {
            if (setupActions == null || setupActions.Length == 0)
                throw new ArgumentException("Fields is empty");
            foreach (var setupAction in setupActions)
            {
                setupAction(this);
            }
            return this;
        }

        internal FieldBuilder FindField(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty");
            FieldBuilder builder;
            if (_fields.IsValueCreated && _fields.Value.TryGetValue(name, out builder))
                return builder;
            return null;
        }

        public FieldBuilder Field(string name, DbType? dbType=null, int? length=null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty");
            FieldBuilder builder = FindField(name);
            if (builder == null)
            {
                builder = new FieldBuilder(this);
                _fields.Value.Add(name, builder);
            }
            if (dbType.HasValue)
                builder.HasType(dbType.Value, length ?? -1);
            return builder;
        }

        public IndexBuilder Index(string name)
        {
            return IndexImplement(name, () => new IndexBuilder());
        }

        protected IndexBuilder IndexImplement(string name, Func<IndexBuilder> builderFactory)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty");
            IndexBuilder builder;
            if (_indexes.IsValueCreated && _indexes.Value.TryGetValue(name, out builder))
                return builder;
            builder = builderFactory();
            _indexes.Value.Add(name, builder);
            return builder;
        }

        protected override void Validate(string clrName)
        {
            base.Validate(clrName);
            if (_fields.IsValueCreated == false)
                throw new InvalidOperationException($"Need fields for table {clrName}");
            foreach (var fieldBuilder in _fields.Value.Values)
            {
                fieldBuilder.Validate();
            }
        }

        public AssociationBuilder Association(string foreignKey, string principalTable, string principalKey)
            =>AssociationImplement(foreignKey, principalTable, principalKey, ()=>new AssociationBuilder());

        protected AssociationBuilder AssociationImplement(
            string foreignKey, 
            string principalTable, 
            string principalKey,
            Func<AssociationBuilder> builderFactory)
        {
            if (string.IsNullOrWhiteSpace(foreignKey))
                throw new ArgumentException("Need foreign key field name.");
            if (string.IsNullOrWhiteSpace(principalTable))
                throw new ArgumentException("Need refeference table name.");
            var key = new Tuple<string, string, string>(foreignKey, principalTable, principalKey);
            AssociationBuilder buider;
            if (_associations.IsValueCreated && _associations.Value.TryGetValue(key, out buider))
                return buider;
            buider = builderFactory();
            _associations.Value.Add(key, buider);
            return buider;
        }

        public TableBuilder With(ProviderName provider, string value)
        {
            if (WithDict.ContainsKey(provider))
                WithDict[provider] = value;
            else
                WithDict.Add(provider, value);
            return this;
        }

        internal ITable Build(string name, IModel model)
        {
            Validate(name);
            var table = new Table(name, model);
            if (string.IsNullOrWhiteSpace(Name)==false)
                table.DbName = Name;

            if (string.IsNullOrWhiteSpace(Schema) == false)
                table.Schema = Schema;

            var builder2field = _fields.Value.ToDictionary(_ => _.Value, _ => _.Value.Build(_.Key, table));
            table.Fields = builder2field.Values;

            if(PrimaryKey!=null)
                table.PrimaryKey = PrimaryKey.Build(table, fb => builder2field[fb]);

            table.With = new ReadOnlyDictionary<ProviderName, string>(WithDict);

            table.Indexes = _indexes.IsValueCreated
                ? _indexes.Value.Select(_ => _.Value.Build(_.Key, table)).ToArray()
                : Enumerable.Empty<IIndex>();
            return table;
        }

        internal void BuildAssociations(ITable itable, IModel model)
        {
            var table = itable as Table;
            if (table != null && _associations.IsValueCreated)
            {
                var associationsList = new List<IAssociation>(_associations.Value.Count);
                foreach (var item in _associations.Value)
                {
                    var fkField = itable.FindField(item.Key.Item1);
                    if (fkField == null)
                        throw new InvalidOperationException($"Field {item.Key.Item1} not found in table {itable.ClrName}");
                    var principalTable = model.FindTable(item.Key.Item2);
                    if (principalTable == null)
                        throw new InvalidOperationException($"Table {item.Key.Item2} not found");
                    IField principalKey;
                    if (string.IsNullOrWhiteSpace(item.Key.Item3))
                    {
                        if (principalTable.PrimaryKey == null || principalTable.PrimaryKey.Items.Any() == false)
                            throw new InvalidOperationException($"Primary key for table {principalTable.ClrName} not found");
                        if (principalTable.PrimaryKey.Items.Count() > 1)
                            throw new NotSupportedException($"{principalTable.ClrName}. Composite primary keys not supported");
                        principalKey = principalTable.PrimaryKey.Items.Single();
                    }
                    else {
                        principalKey = principalTable.FindField(item.Key.Item3);
                        if (principalKey == null)
                            throw new InvalidOperationException($"Field {principalKey} not found in table {principalTable.ClrName}");
                    }
                    var association = item.Value.Build(fkField, principalKey);
                    associationsList.Add(association);
                }
                table.Associations = new ReadOnlyCollection<IAssociation>(associationsList);
            }
            else
                table.Associations = Enumerable.Empty<IAssociation>();
        }
    }
}