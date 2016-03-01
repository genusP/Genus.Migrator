using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Migrator.Model.Internal;

namespace Genus.Migrator.Model.Builder
{
    public class IndexBuilder
    {
        Dictionary<string, bool> _fileds = new Dictionary<string, bool>();
        public IndexBuilder OnColumn(string field, bool descending = false)
        {
            if (_fileds.ContainsKey(field))
                _fileds[field] = descending;
            else
                _fileds.Add(field, descending);
            return this;
        }

        bool isUnique { get; set; }

        public IndexBuilder IsUnique()
        {
            isUnique = true;
            return this;
        }

        internal void Validate()
        {
            if (_fileds.Any() == false)
                throw new InvalidOperationException("Add fields to index");
        }

        internal IIndex Build(string name, ITable table)
        {
            Validate();
            var index = new Index(name, table)
            {
                IsUnique = isUnique,
                Fields = _fileds.Select(f =>
                            new IndexItem
                            {
                                Field = table.FindField(f.Key),
                                Decending = f.Value
                            }).ToArray()
            };
            return index;
        }
    }
}
