using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class FieldsBuilder
    {
        readonly IList<FieldBuilder> _fieldBuilders;
        public FieldsBuilder(IList<FieldBuilder> fieldBuilders)
        {
            if (fieldBuilders == null)
                throw new ArgumentNullException(nameof(fieldBuilders));
            _fieldBuilders = fieldBuilders;
        }
        public FieldBuilder Field(string name, DbType dbType, int? length =null, bool nullable=true, bool identity=false)
        {
            var res = new FieldBuilder(
                new AddField
                {
                    Name = name,
                    Type = dbType,
                    Length = length,
                    IsNullable = nullable,
                    IsIdentity = identity
                }
                );
            _fieldBuilders.Add(res);
            return res;
        }
    }
}
