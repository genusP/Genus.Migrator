using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class ClrTypeConverter
    {
        private static readonly IDictionary<Type, DbType> _dbTypeMap = new Dictionary<Type, DbType> {
                {typeof(char),            DbType.StringFixedLength},
                {typeof(string),          DbType.String           },
                {typeof(decimal),         DbType.Decimal          },
                {typeof(DateTime),        DbType.DateTime        },
                {typeof(DateTimeOffset),  DbType.DateTimeOffset   },
                {typeof(TimeSpan),        DbType.Time             },
                {typeof(byte[]),          DbType.Binary           },
                {typeof(Guid),            DbType.Guid             },
                {typeof(object),          DbType.Object           },
                {typeof(bool),            DbType.Boolean          },
                {typeof(sbyte),           DbType.SByte            },
                {typeof(short),           DbType.Int16            },
                {typeof(int),             DbType.Int32            },
                {typeof(long),            DbType.Int64            },
                {typeof(byte),            DbType.Byte             },
                {typeof(ushort),          DbType.UInt16           },
                {typeof(uint),            DbType.UInt32           },
                {typeof(ulong),           DbType.UInt64           },
                {typeof(float),           DbType.Single           },
                {typeof(double),          DbType.Double           }
        };

        public static void MapDbTypeToClrType(DbType dbType, Type clrType)
        {
            if (_dbTypeMap.ContainsKey(clrType))
                _dbTypeMap[clrType] = dbType;
            else
                _dbTypeMap.Add(clrType, dbType);
        }

        public static bool IsConvertable(Type clrType)
            => _dbTypeMap.ContainsKey(clrType);
        

        public static DbType ConvertClrTypeToDbType(Type clrType)
        {
            DbType res;
            if (_dbTypeMap.TryGetValue(clrType, out res) == false)
                throw new InvalidCastException($"Correct DbType for {clrType.FullName} not found. Use {nameof(DataAnnotationBuilderExtension)}.{nameof(MapDbTypeToClrType)} for setup mapping.");
            return res;
        }
    }
}
