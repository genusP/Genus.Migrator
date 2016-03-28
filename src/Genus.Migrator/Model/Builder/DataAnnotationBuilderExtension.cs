using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public static class DataAnnotationBuilderExtension
    { 

        public static TableBuilder<TEntity> TableFromDA<TEntity>(this ModelBuilder modelBuilder)
        {
            return modelBuilder.Table<TEntity>()
                .NameFromDA()
                .FieldsFromDA()
                .PrimaryKeyFromDA()
                .AssociationsFromDA();
        }

        public static TableBuilder<TEntity> NameFromDA<TEntity>(this TableBuilder<TEntity> tableBuilder)
        {
            var tableAttribute = typeof(TEntity).GetTypeInfo().GetCustomAttribute<TableAttribute>(true);
            if (tableAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(tableAttribute.Name))
                    tableBuilder.HasName(tableAttribute.Name);
                if (!string.IsNullOrWhiteSpace(tableAttribute.Schema))
                    tableBuilder.HasSchema(tableAttribute.Schema);
            }
            return tableBuilder;
        }

        public static TableBuilder<TEntity> FieldsFromDA<TEntity>(this TableBuilder<TEntity> tableBuilder)
        {
            var keys = new List<FieldBuilder>();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                var notmapped = property.GetCustomAttribute<NotMappedAttribute>();
                if (notmapped == null)
                {
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

                    if (ClrTypeConverter.IsConvertable(dataType))
                    {
                        var maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
                        var fieldBuilder = tableBuilder.Field(property.Name, ClrTypeConverter.ConvertClrTypeToDbType(dataType), maxLength?.Length ?? -1);

                        var column = property.GetCustomAttribute<ColumnAttribute>();
                        if (column != null && string.IsNullOrWhiteSpace(column.Name) == false)
                            fieldBuilder.HasName(column.Name);

                        var required = property.GetCustomAttribute<RequiredAttribute>();
                        if (required == null && nullable)
                            fieldBuilder.AsNullable();

                        var databaseGenerated = property.GetCustomAttribute<DatabaseGeneratedAttribute>();
                        if (databaseGenerated != null && databaseGenerated.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                            fieldBuilder.AsIdentity();
                    }
                }
            }
            return tableBuilder;
        }

        public static TableBuilder<TEntity> PrimaryKeyFromDA<TEntity>(this TableBuilder<TEntity> tableBuilder)
        {
            var keys = GetKeys<TEntity>().ToArray();
            if (keys.Any())
                tableBuilder.HasKey(keys);
            return tableBuilder;
        }

        private static MethodInfo _daassociationMemberInfo 
            = typeof(DataAnnotationBuilderExtension).GetMethod(nameof(AddAssociation), BindingFlags.NonPublic | BindingFlags.Static);

        public static TableBuilder<TEntity> AssociationsFromDA<TEntity>(this TableBuilder<TEntity> tableBuilder)
        {
            foreach (var property in typeof(TEntity).GetProperties())
            {
                var fkAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (fkAttribute != null)
                {
                    var dataType = property.PropertyType;
                    if (dataType.GetTypeInfo().IsGenericType
                        && dataType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && typeof(IEnumerable<>).IsAssignableFrom(dataType.GetGenericTypeDefinition()))
                        dataType = dataType.GetGenericArguments()[0];

                    var fkProperty = fkAttribute.Name;

                    var addAssociationMethod = _daassociationMemberInfo.MakeGenericMethod(typeof(TEntity), dataType);
                    var associationBuilder = (AssociationBuilder)addAssociationMethod.Invoke(null, new object[] { fkProperty, null, tableBuilder });
                    if (associationBuilder != null)
                    {
                        var inverce = GetInverce<TEntity>(dataType, property.Name);
                        associationBuilder.WithNavigation(property.Name, inverce);

                        associationBuilder.AsForeignKey($"FK_{typeof(TEntity).Name}__{dataType.Name}__{fkProperty}");
                    }
                }
            }
            return tableBuilder;
        }

        private static AssociationBuilder AddAssociation<TEntity, TPrincipal>(string fkPropertyName, string invercePropertyName, TableBuilder<TEntity> tableBuilder)
        {
            var foreignKey = GetFieldExpression<TEntity>(fkPropertyName);
            var keyFields = GetKeys<TPrincipal>().ToArray();
            if (keyFields.Count() == 1)
            {
                var principalKeyExp = GetFieldExpression<TPrincipal>(keyFields.First());
                if (principalKeyExp != null)
                    return tableBuilder.Association<TPrincipal>(foreignKey, principalKeyExp);
            }
            return null;
        }

        private static string GetInverce<TEnity>(Type refType, string fkPropertyName)
        {
            return (
                from p in refType.GetProperties()
                where p.GetCustomAttribute<InversePropertyAttribute>()?.Property == fkPropertyName
                      && (
                          typeof(TEnity) == p.PropertyType
                          || (
                                p.PropertyType.GetTypeInfo().IsGenericType
                                && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                && p.PropertyType.GetGenericArguments()[0] == typeof(TEnity)
                             )
                          || typeof(IEnumerable<TEnity>).IsAssignableFrom(p.PropertyType)
                          )
                select p.Name
            ).SingleOrDefault();
        }
        
        private static Expression<Func<TEntity, object>> GetFieldExpression<TEntity>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TEntity));
            return Expression.Lambda<Func<TEntity, object>>(
                Expression.Convert(
                    Expression.Property(param, propertyName),
                    typeof(object)),
                param
                );
        }

        private static IEnumerable<string> GetKeys<TEntity>()
            => from p in typeof(TEntity).GetProperties()
               let atr = p.GetCustomAttribute<KeyAttribute>()
               where atr != null
               select p.Name;
    }
}
