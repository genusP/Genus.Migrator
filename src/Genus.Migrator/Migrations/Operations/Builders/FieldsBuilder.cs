using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Operations.Builders
{
    public class FieldsBuilder
    {
        public FieldBuilder Field(DbType dbType, int? length =null, bool nullable=true, bool identity=false)
        {
            var res = new FieldBuilder(
                new AddField
                {
                    Type = dbType,
                    Length = length,
                    IsNullable = nullable,
                    IsIdentity = identity
                }
                );
            return res;
        }
    }
}
