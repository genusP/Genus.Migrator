using Genus.Migrator.Model.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Genus.Migrator.Model.Builder
{
    public class PrimaryKeyBuilder
    {
        public PrimaryKeyBuilder(params FieldBuilder[] fieldBuilders)
        {
            FieldBuilders = fieldBuilders;
        }

        public PrimaryKeyBuilder(TableBuilder tableBuilder, params string[] fields)
            :this(FieldNameToFieldBuilder(tableBuilder, fields))
        {

        }

        private static FieldBuilder[] FieldNameToFieldBuilder(TableBuilder tableBuilder, string[] fields)
        {
            var fieldBuilders = new FieldBuilder[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                var fieldName = fields[i];
                var fb = tableBuilder.FindField(fieldName);
                if (fb == null)
                    throw new IndexOutOfRangeException($"Field {fieldName} not found");
                fieldBuilders[i] = fb;
            }
            return fieldBuilders;
        }

        protected string Name { get; set; }

        protected IEnumerable<FieldBuilder> FieldBuilders { get; set; }

        public PrimaryKeyBuilder HasName(string name)
        {
            Name = name;
            return this;
        }

        internal IPrimaryKey Build(ITable table, Func<FieldBuilder,IField> fieldResolver)
        {
            if (FieldBuilders.Any() == false)
                return null;
            var primaryKey = new PrimaryKey {
                DbName = string.IsNullOrEmpty(Name)
                        ?"PK_"+table.DbName
                        : Name,
                Items = FieldBuilders.Select(_ => fieldResolver(_)).ToArray()
            };
            return primaryKey;
        }
    }
}