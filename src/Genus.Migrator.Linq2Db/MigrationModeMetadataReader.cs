using LinqToDB.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Genus.Migrator.Model;
using LinqToDB;
using System.Data;
using System.Collections.ObjectModel;

namespace Genus.Migrator.Linq2Db
{
    public class MigrationModelMetadataReader : IMetadataReader
    {
        readonly IModel _model;

        public MigrationModelMetadataReader(IModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            _model = model;
        }

        public T[] GetAttributes<T>(MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            var queryAtrType = typeof(T);

            if (queryAtrType == typeof(LinqToDB.Mapping.ColumnAttribute))
            {
                var field = GetField(memberInfo);
                if (field != null)
                    return new T[] {
                        new LinqToDB.Mapping.ColumnAttribute(field.DbName) {
                            CanBeNull = field.IsNullable,
                            DataType = DbTypeToDataType(field.DataType),
                            Length = field.Length,
                            IsIdentity = field.IsIdentity
                        } as T
                    };
            }
            if (queryAtrType == typeof(LinqToDB.Mapping.PrimaryKeyAttribute))
            {
                var table = _model.Tables.FirstOrDefault(t => t.ClrName == memberInfo.DeclaringType.FullName);
                if (table != null && table.PrimaryKey != null)
                {
                    var order = FindIndex(table.PrimaryKey.Items, f => f.ClrName == memberInfo.Name);
                    if (order >= 0)
                        return new T[] { new LinqToDB.Mapping.PrimaryKeyAttribute(order) as T };
                }
            }
            
            if(queryAtrType == typeof(LinqToDB.Mapping.NullableAttribute))
            {
                var field = GetField(memberInfo);
                if (field != null && field.IsNullable)
                    return new[] { new LinqToDB.Mapping.NullableAttribute() as T };
            }

            if (queryAtrType == typeof(LinqToDB.Mapping.AssociationAttribute))
            {
                var prop = memberInfo as PropertyInfo;
                if (prop != null)
                {
                    var tableName = memberInfo.ReflectedType.FullName;
                    var association = _model.FindTable(tableName)
                                            ?.Associations
                                            ?.SingleOrDefault(a => a.DependentNavigation == prop.Name);
                    if (association != null)
                        return new T[] {
                            new LinqToDB.Mapping.AssociationAttribute() {
                                ThisKey = association.Field.ClrName,
                                OtherKey = association.ReferenceField.ClrName
                            } as T
                        };

                    var backRef = (
                                  from t in _model.Tables
                                  from a in t.Associations
                                  where a.ReferenceField.Table.ClrName == tableName
                                        && a.PrincipalNavigation == prop.Name
                                  select a
                                  ).SingleOrDefault();
                    if (backRef!=null)
                        return new T[] {
                            new LinqToDB.Mapping.AssociationAttribute() {
                                ThisKey = backRef.ReferenceField.ClrName,
                                OtherKey = backRef.Field.ClrName,
                                IsBackReference = true
                            } as T
                        };
                }
            }

            return new T[0];
        }

        private static readonly IReadOnlyDictionary<DbType, DataType> _dbTypes 
                    = new ReadOnlyDictionary<DbType, DataType>( 
                        new Dictionary<DbType, DataType> {
                            {DbType.AnsiStringFixedLength,  DataType.Char           },
                            {DbType.AnsiString           ,  DataType.VarChar        },
                            {DbType.StringFixedLength    ,  DataType.NChar          },
                            {DbType.String               ,  DataType.NVarChar       },
                            {DbType.Binary               ,  DataType.VarBinary      },
                            {DbType.Boolean              ,  DataType.Boolean        },
                            {DbType.SByte                ,  DataType.SByte          },
                            {DbType.Int16                ,  DataType.Int16          },
                            {DbType.Int32                ,  DataType.Int32          },
                            {DbType.Int64                ,  DataType.Int64          },
                            {DbType.Byte                 ,  DataType.Byte           },
                            {DbType.UInt16               ,  DataType.UInt16         },
                            {DbType.UInt32               ,  DataType.UInt32         },
                            {DbType.UInt64               ,  DataType.UInt64         },
                            {DbType.Single               ,  DataType.Single         },
                            {DbType.Double               ,  DataType.Double         },
                            {DbType.Decimal              ,  DataType.Decimal        },
                            {DbType.Guid                 ,  DataType.Guid           },
                            {DbType.Date                 ,  DataType.Date           },
                            {DbType.Time                 ,  DataType.Time           },
                            {DbType.DateTime             ,  DataType.DateTime       },
                            {DbType.DateTime2            ,  DataType.DateTime2      },
                            {DbType.DateTimeOffset       ,  DataType.DateTimeOffset },
                            {DbType.Object               ,  DataType.Variant        },
                            {DbType.VarNumeric           ,  DataType.VarNumeric     }
                            });


        private DataType DbTypeToDataType(DbType dataType)
        {
            return _dbTypes[dataType];
        }

        public static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }


        public IField GetField(MemberInfo memberInfo)
        {
            return _model.FindTable(memberInfo.ReflectedType.FullName)?.FindField(memberInfo.Name);
        }

        public T[] GetAttributes<T>(Type type, bool inherit = true) where T : Attribute
        {
            if (typeof(T) == typeof(LinqToDB.Mapping.TableAttribute))
            {
                var table = _model.Tables.FirstOrDefault(t => t.ClrName == type.FullName);
                var view = _model.Views.FirstOrDefault(v => v.ClrName == type.FullName);
                var dbobj = table as IDbObject ?? view as IDbObject;

                if (dbobj != null)
                    return new T[] {
                        new LinqToDB.Mapping.TableAttribute(dbobj.DbName) { Schema = dbobj.Schema } as T
                    };
            }
            return new T[0];
        }
    }
}
