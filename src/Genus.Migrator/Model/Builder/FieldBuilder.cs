using Genus.Migrator.Model.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Genus.Migrator.Model.Builder
{
    public class FieldBuilder
    {
        private readonly TableBuilder _entityBuilder;
        public FieldBuilder(TableBuilder entityBuilder)
        {
            if(entityBuilder == null)
                throw new ArgumentNullException(nameof(entityBuilder));
            _entityBuilder = entityBuilder;
        }

        protected string DbName { get; private set; }

        protected DbType DataType { get; private set; }

        protected bool IsIdentity { get; private set; }

        protected int Length { get; private set; }

        protected bool IsNullable { get; private set; }

        protected Dictionary<ProviderName, string> Collation { get; } = new Dictionary<ProviderName, string>();
        protected Dictionary<ProviderName, string> Default { get; } = new Dictionary<ProviderName, string>();
        protected List<IAssociation> ForeignKeys { get; } = new List<IAssociation>();

        public FieldBuilder HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));
            DbName = name;
            return this;
        }

        public FieldBuilder HasType(DbType type, int length=-1)
        {
            DataType = type;
            Length = length;
            return this;
        }

        public FieldBuilder AsIdentity()
        {
            IsIdentity = true;
            IsNullable = false;
            return this;
        }

        public FieldBuilder AsNullable()
        {
            IsNullable = true;
            IsIdentity = false;
            return this;
        }

        public FieldBuilder IsPrimaryKey()
        {
            _entityBuilder.HasKey(this);
            return this;
        }

        public FieldBuilder HasCollation(ProviderName provider, string collation)
        {
            if (Collation.ContainsKey(provider))
                Collation[provider] = collation;
            else
                Collation.Add(provider, collation);
            return this;
        }

        public FieldBuilder HasDefault(ProviderName provider, string @default)
        {
            if (Default.ContainsKey(provider))
                Default[provider] = @default;
            else
                Default.Add(provider, @default);
            return this;
        }

        public void Validate() { }

        internal IField Build(string name, ITable table)
        {
            var res = new Field(
                name,
                table,
                DataType,
                Length,
                new ReadOnlyDictionary<ProviderName, string>(Collation),
                new ReadOnlyDictionary<ProviderName, string>(Default)
                );
            if (!string.IsNullOrWhiteSpace(DbName))
                res.DbName = DbName;

            res.IsIdentity = IsIdentity == true;
            res.IsNullable = IsNullable == true;

            return res;
        }
    }
}