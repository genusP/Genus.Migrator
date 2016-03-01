using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Migrator.Model.Builder;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;

namespace Genus.Migrator.Model.Conventions
{
    public class DataAnnotationConvention : ITableConvention
    {
        public void Apply<TEntity>(TableBuilder<TEntity> tableBuilder, ModelBuilder modelBuilder)
        {
            var entityType = typeof(TEntity);
            ApplyTableAttribute(entityType, tableBuilder);
            var keys = ApplyFields(entityType, tableBuilder, modelBuilder);
            tableBuilder.HasKey(keys);

            ApplyAssociations<TEntity>(tableBuilder, modelBuilder);
        }

        private FieldBuilder[] ApplyFields<TEntity>(Type entityType, TableBuilder<TEntity> tableBuilder, ModelBuilder modelBuilder)
        {
            var keys = new List<FieldBuilder>();
            foreach (var property in entityType.GetProperties())
            {
                var fb = ApplyField(property, tableBuilder);
                if (fb != null
                    && property.GetCustomAttribute<KeyAttribute>() != null)
                    keys.Add(fb);
            }
            return keys.ToArray();
        }

        private FieldBuilder ApplyField(PropertyInfo property, TableBuilder tableBuilder)
        {
            var notmapped = property.GetCustomAttribute<NotMappedAttribute>();
            if (notmapped != null)
                return null;
            var dataType = property.PropertyType;
            var nullable = false;
            if (dataType.GetTypeInfo().IsGenericType)
                if (dataType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    dataType = dataType.GenericTypeArguments[0];
                    nullable = true;
                }
            if (dataType == typeof(string))
                nullable = true;

            if (_dbTypeMap.ContainsKey(dataType)==false)
                return null;

            var fieldBuilder = tableBuilder.Field(property.Name);

            var column = property.GetCustomAttribute<ColumnAttribute>();
            if (column != null && string.IsNullOrWhiteSpace(column.Name) == false)
                fieldBuilder.HasName(column.Name);

            ApplyFieldDataType(property, fieldBuilder, dataType);

            var required = property.GetCustomAttribute<RequiredAttribute>();
            if (required == null && nullable)
                fieldBuilder.AsNullable();

            var databaseGenerated = property.GetCustomAttribute<DatabaseGeneratedAttribute>();
            if (databaseGenerated != null && databaseGenerated.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                fieldBuilder.AsIdentity();

            return fieldBuilder;
        }

        private void ApplyFieldDataType(PropertyInfo property, FieldBuilder fieldBuilder, Type dataType)
        {
            var maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
            fieldBuilder.HasType( ConvertClrTypeToDbType(dataType), maxLength?.Length ?? -1);            
        }

        private void ApplyTableAttribute(Type entityType, TableBuilder tableBuilder)
        {
            var tableAttribute = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>(true);
            if (tableAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(tableAttribute.Name))
                    tableBuilder.HasName(tableAttribute.Name);
                if (!string.IsNullOrWhiteSpace(tableAttribute.Schema))
                    tableBuilder.HasSchema(tableAttribute.Schema);
            }
        }

        private void ApplyAssociations<TEntity>( TableBuilder<TEntity> tableBuilder, ModelBuilder modelBuilder)
        {
            foreach (var property in typeof(TEntity).GetProperties())
            {
                ApplyAssociation<TEntity>(property, tableBuilder, modelBuilder);
            }
        }

        private void ApplyAssociation<TEntity>(PropertyInfo property, TableBuilder<TEntity> tableBuilder, ModelBuilder modelBuilder)
        {
            var fkAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
            if(fkAttribute!=null)
            {
                var dataType = property.PropertyType;
                if (dataType.GetTypeInfo().IsGenericType
                    && dataType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && typeof(IEnumerable<>).IsAssignableFrom(dataType.GetGenericTypeDefinition()))
                    dataType = dataType.GetGenericArguments()[0];

                var fkProperty = fkAttribute.Name;

                var addAssociationGenMethod = GetType().GetMethod(nameof(AddAssociation), BindingFlags.NonPublic | BindingFlags.Instance);
                var addAssociationMethod = addAssociationGenMethod.MakeGenericMethod(typeof(TEntity), dataType);
                var associationBuilder = (AssociationBuilder) addAssociationMethod.Invoke(this, new object[] { fkProperty, null, tableBuilder });

                var inverce = GetInverce<TEntity>(dataType, property.Name);
                associationBuilder.WithNavigation(property.Name, inverce);

                associationBuilder.AsForeignKey($"FK_{typeof(TEntity).Name}__{dataType.Name}__{fkProperty}");
            }
        }

        private string GetInverce<TEnity>(Type refType, string fkPropertyName)
        {
            return (
                from p in refType.GetProperties()
                where p.GetCustomAttribute<InversePropertyAttribute>()?.Property == fkPropertyName
                      && (
                          typeof(TEnity) == p.PropertyType
                          || (
                                p.PropertyType.GetTypeInfo().IsGenericType 
                                && p.PropertyType.GetGenericTypeDefinition()==typeof(Nullable<>) 
                                && p.PropertyType.GetGenericArguments()[0] == typeof(TEnity)
                             )
                          || typeof(IEnumerable<TEnity>).IsAssignableFrom(p.PropertyType)
                          )
                select p.Name
            ).SingleOrDefault();
        }

        private AssociationBuilder AddAssociation<TEntity, TPrincipal>(string fkPropertyName, string invercePropertyName, TableBuilder<TEntity> tableBuilder)
        {
            var foreignKey = GetFieldExpression<TEntity>(fkPropertyName);
            var principalKey = !string.IsNullOrEmpty(invercePropertyName) 
                                    ?GetFieldExpression<TPrincipal>(invercePropertyName)
                                    :null;
            return tableBuilder.Association<TPrincipal>(foreignKey, principalKey);
        }

        private Expression<Func<TEntity, object>> GetFieldExpression<TEntity>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TEntity));
            return Expression.Lambda<Func<TEntity, object>>(
                Expression.Convert(
                    Expression.Property(param, propertyName),
                    typeof(object)),
                param
                );
        }

        protected virtual DbType ConvertClrTypeToDbType(Type clrType)
        {
            DbType res;
            if (_dbTypeMap.TryGetValue(clrType, out res) == false)
                throw new InvalidCastException($"Correct DbType for {clrType.FullName} not found. Use {nameof(DataAnnotationConvention)}.{nameof(MapDbTypeToClrType)} for setup mapping.");
            return res;
        }

        private static readonly IDictionary<Type, DbType> _dbTypeMap = new Dictionary<Type, DbType> {
                {typeof(char),            DbType.StringFixedLength},
                {typeof(string),          DbType.String           },
                {typeof(decimal),         DbType.Decimal          },
                {typeof(DateTime),        DbType.DateTime2        },
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
    }
}
