using Genus.Migrator.Migrations.Operations;
using Genus.Migrator.Migrations.Operations.Builders;
using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design.Internal
{
    public class ModelComparer : IModelComparer
    {
        public virtual IEnumerable<MigrationOperation> CompareModel(IModel sourceModel, IModel targetModel)
        {
            return
            CompareTables(sourceModel?.Tables, targetModel?.Tables)
                .Concat(
                    CompareViews(sourceModel?.Views, targetModel?.Views))
                .Concat(
                    CompareFunctions(sourceModel?.Functions, targetModel?.Functions))
                .OrderBy(o=>o.OrderHint);
        }

        protected virtual IEnumerable<MigrationOperation> CompareTables(IEnumerable<ITable> tables1, IEnumerable<ITable> tables2)
        {
            var pairs = FullJoin(tables1, v => v.ClrName, tables2, v => v.ClrName);

            foreach (var item in pairs)
            {
                var current = item.Item1;
                var target = item.Item2;
                if (target== null)
                    yield return new DropTable
                    {
                        Schema    = current.Schema,
                        TableName = current.DbName
                    };
                else if (current == null)
                {
                    var ct = new CreateTable
                    {
                        Schema     = target.Schema,
                        TableName  = target.DbName,
                        Fields     = CompareFields(Enumerable.Empty<IField>(), target.Fields).OfType<AddField>(),
                        PrimaryKey = target.PrimaryKey==null
                                        ?null
                                        :new AddPrimaryKey
                                        {
                                            PKName = target.PrimaryKey.DbName,
                                            Fields = target.PrimaryKey.Items.Select(f=>f.DbName).ToArray()
                                        },
                    };
                    yield return ct;
                    foreach (var operation in CompareIndexes(Enumerable.Empty<IIndex>(), target.Indexes))
                        yield return operation;
                    foreach (var operation in CompareAssociations(Enumerable.Empty<IAssociation>(), target.Associations))
                        yield return operation;
                }
                else
                {
                    var isChangeSchema = !string.Equals(current.Schema, target.Schema, StringComparison.OrdinalIgnoreCase);
                    var isChangeName = !string.Equals(current.DbName, target.DbName, StringComparison.OrdinalIgnoreCase);
                    if (isChangeSchema || isChangeName)
                        yield return new RenameTable
                        {
                            Schema = current.Schema,
                            TableName = current.DbName,
                            NewSchema = isChangeSchema ? target.Schema : null,
                            NewTableName= isChangeName ? target.DbName : null
                        };
                    foreach (var operation in CompareFields(current.Fields, target.Fields))
                        yield return operation;

                    foreach (var opertion in ComparePrimaryKey(current.PrimaryKey, target.PrimaryKey))
                        yield return opertion;

                    foreach (var opertion in CompareIndexes(current.Indexes, target.Indexes))
                        yield return opertion;

                    foreach (var operation in CompareAssociations(current.Associations, target.Associations))
                        yield return operation;
                }
            }
        }

        protected virtual IEnumerable<MigrationOperation> CompareFields(IEnumerable<IField> field1, IEnumerable<IField> fields2)
        {
            var pairs = FullJoin(field1, f => f.ClrName, fields2, f => f.ClrName);
            foreach (var item in pairs)
            {
                var current = item.Item1;
                var target = item.Item2;
                if (current == null)
                {
                    var af = new AddField
                    {
                        IsIdentity = target.IsIdentity,
                        IsNullable = target.IsNullable,
                        Length = target.Length,
                        Name = target.DbName,
                        Schema = target.Table.Schema,
                        TableName = target.Table.DbName,
                        Type = target.DataType
                    };
                    var builder = new FieldBuilder(af);
                    foreach (var def in target.Default)
                        builder.Default(def.Key, def.Value);
                    foreach (var collation in target.Collation)
                        builder.Collation(collation.Key, collation.Value);
                    yield return af;
                }
                else if(target == null)
                {
                    yield return new DropField
                    {
                        Schema    = current.Table.Schema,
                        TableName = current.Table.DbName,
                        FieldName = current.DbName
                    };
                }
                else
                {
                    if (!string.Equals(current.DbName, target.DbName, StringComparison.OrdinalIgnoreCase))
                        yield return new RenameField
                        {
                            Schema       = target.Table.Schema,
                            TableName    = target.Table.DbName,
                            FieldName    = current.DbName,
                            NewFieldName = target.DbName
                        };
                    var typeChanged      = current.DataType != target.DataType || current.Length!=target.Length;
                    var nullableChanged  = current.IsNullable != target.IsNullable;
                    var identityChanged  = current.IsIdentity != target.IsIdentity;
                    var collationChanged = FullJoin(current.Collation, c => c.Key, target.Collation, c => c.Key).Any(i => i.Item1.Value != i.Item2.Value);
                    var defaultChanged   = FullJoin(current.Default, d => d.Key, target.Default, d => d.Key).Any(i => i.Item1.Value != i.Item2.Value);
                    if(typeChanged
                        || nullableChanged
                        || identityChanged
                        || defaultChanged
                        || collationChanged)
                    {
                        yield return new AlterField
                        {
                            Schema     = target.Table.Schema,
                            TableName  = target.Table.DbName,
                            Name       = target.DbName,
                            IsIdentity = identityChanged ? target.IsIdentity : (bool?)null,
                            IsNullable = nullableChanged ? target.IsNullable : (bool?)null,
                            Type       = typeChanged ? target.DataType : (DbType?)null,
                            Length     = typeChanged ? target.Length : (int?)null
                        };
                    }
                }
            }
        }

        protected virtual IEnumerable<MigrationOperation> ComparePrimaryKey(IPrimaryKey current, IPrimaryKey target)
        {
            if (current == null)
            {
                var targetPkTable = target.Items.First().Table;
                yield return new AddPrimaryKey
                {
                    Schema = targetPkTable.Schema,
                    TableName = targetPkTable.DbName,
                    PKName = target.DbName,
                    Fields = target.Items.Select(f => f.DbName).ToArray()
                };
            }
            else if(target == null)
            {
                var pkTable = current.Items.First().Table;
                yield return new DropPrimaryKey
                {
                    PrimaryKeyName = current.DbName,
                    Schema = pkTable.Schema,
                    TableName = pkTable.DbName
                };
            }
            else
            {
                var currentPkTable = current.Items.First().Table;
                var targetPkTable = target.Items.First().Table;
                bool isItemsChanged = !current.Items.Select(_=>_.DbName).SequenceEqual(target.Items.Select(_ => _.DbName));
                if (current.DbName != target.DbName||isItemsChanged)
                {
                    yield return new DropPrimaryKey
                    {
                        Schema = currentPkTable.Schema,
                        TableName = currentPkTable.DbName,
                        PrimaryKeyName = current.DbName
                    };
                    yield return new AddPrimaryKey
                    {
                        Schema = targetPkTable.Schema,
                        TableName = targetPkTable.DbName,
                        PKName = target.DbName,
                        Fields = target.Items.Select(f => f.DbName).ToArray()
                    };
                }
            }
        }

        private IEnumerable<MigrationOperation> CompareAssociations(IEnumerable<IAssociation> associations1, IEnumerable<IAssociation> associations2)
        {
            var pairs = FullJoin(
                associations1.Where(a=>!string.IsNullOrWhiteSpace(a.ForeignKeyName)), 
                a => a.ForeignKeyName, 
                associations2.Where(a => !string.IsNullOrWhiteSpace(a.ForeignKeyName)), 
                a => a.ForeignKeyName);

            foreach (var item in pairs)
            {
                var current = item.Item1;
                var target = item.Item2;
                var comparer = new AssociationEqualityComparer();
                var associationChanged = current != null && target != null
                    ? !comparer.Equals(current, target)
                    : false;
                if (target == null || associationChanged)
                    yield return new DropForeignKey
                    {
                        Schema = current.Field.Table.Schema,
                        TableName = current.Field.Table.DbName,
                        ForeignKeyName = current.ForeignKeyName
                    };
                if (current == null || associationChanged)
                    yield return new AddForeignKey
                    {
                        Schema = target.Field.Table.Schema,
                        TableName = target.Field.Table.DbName,
                        ForeignKeyName = target.ForeignKeyName,
                        FieldName = target.Field.DbName,
                        PrincipalSchema = target.ReferenceField.Table.Schema,
                        PrincipalTable = target.ReferenceField.Table.DbName,
                        PrincipalField = target.ReferenceField.DbName,
                        OnUpdate = target.OnUpdateAction,
                        OnDelete = target.OnDeleteAction
                    };
            }
        }

        private IEnumerable<MigrationOperation> CompareIndexes(IEnumerable<IIndex> indexes1, IEnumerable<IIndex> indexes2)
        {
            var pairs = FullJoin(indexes1, i => i.DbName, indexes2, i => i.DbName);
            var currentForCompareByFields = pairs.Where(i=>i.Item2==null).Select(i=>i.Item1).ToList();
            var targetForCompareByFields = pairs.Where(i => i.Item1 == null).Select(i => i.Item2).ToList();
            var indexItemEC = new IndexItemEqualityComparer();

            foreach (var item in pairs.Where(i=>i.Item1!=null && i.Item2!=null ))
            {
                var current = item.Item1;
                var target = item.Item2;
                if(current.IsUnique!=target.IsUnique
                    || !current.Fields.SequenceEqual(target.Fields, indexItemEC))
                {
                    yield return new DropIndex
                    {
                        Schema = current.Table.Schema,
                        TableName = current.Table.DbName,
                        IndexName = current.DbName
                    };
                    yield return new CreateIndex
                    {
                        Schema = target.Table.Schema,
                        TableName = target.Table.DbName,
                        IndexName = target.DbName,
                        IsUnique = target.IsUnique,
                        Fields = target.Fields.Select(f => f.Field.DbName),
                    };
                }
            }
            foreach (var item in currentForCompareByFields)
            {
                var target = targetForCompareByFields.FirstOrDefault(i => item.Fields.SequenceEqual(i.Fields, indexItemEC));
                if (target == null)
                {
                    yield return new DropIndex
                    {
                        Schema    = item.Table.Schema,
                        TableName = item.Table.DbName,
                        IndexName = item.DbName
                    };
                }
                else
                {
                    yield return new RenameIndex
                    {
                        Schema = item.Table.Schema,
                        TableName = item.Table.DbName,
                        IndexName = item.DbName,
                        NewIndexName = target.DbName
                    };
                    targetForCompareByFields.Remove(target);
                }
            }
            foreach (var item in targetForCompareByFields)
            {
                yield return new CreateIndex
                {
                    Schema    = item.Table.Schema,
                    TableName = item.Table.DbName,
                    IndexName = item.DbName,
                    IsUnique  = item.IsUnique,
                    Fields    = item.Fields.Select(f => f.Field.DbName),
                };
            }
        }

        protected IEnumerable<Tuple<T,T>> FullJoin<T, TKey>(IEnumerable<T> src1, Func<T, TKey> key1, IEnumerable<T> src2, Func<T, TKey> key2)
        {
            var dict1 = src1?.ToDictionary(key1);
            var dict2 = src2?.ToDictionary(key2);
            return FullJoin(dict1, dict2);
        }

        protected IEnumerable<Tuple<T,T>> FullJoin<T, TKey>(IDictionary<TKey,T> dict1, IDictionary<TKey, T> dict2)
        {
            if (dict1 == null)
                dict1 = new Dictionary<TKey, T>();
            if (dict2 == null)
                dict2 = new Dictionary<TKey, T>();
            foreach (var item in dict1)
            {
                T val2;
                if (dict2.TryGetValue(item.Key, out val2))
                    dict2.Remove(item.Key);
                yield return new Tuple<T, T>( item.Value, val2 );
            }
            foreach (var item in dict2.Values)
            {
                yield return new Tuple<T, T>(default(T), item);
            }
        }

        protected virtual IEnumerable<MigrationOperation> CompareViews(IEnumerable<IView> views1, IEnumerable<IView> views2)
        {
            var pairs = FullJoin(views1, v => v.ClrName, views2, v => v.ClrName); 

            foreach (var item in pairs)
            {
                if (item.Item2 == null)
                    yield return new DropView
                    {
                        Schema = item.Item1.Schema,
                        ViewName = item.Item1.DbName
                    };
                else if(item.Item1 == null)
                {
                    var cv = new CreateView
                    {
                        Schema = item.Item2.Schema,
                        ViewName = item.Item2.DbName
                    };
                    var b = new ViewBuilder(cv);
                    foreach (var sqlbody in item.Item1.SqlBody)
                    {
                        b.SetScript(sqlbody.Key, sqlbody.Value);
                    };
                    yield return cv;
                }
                else
                {
                    var isChangeSchema = string.Equals(item.Item1.Schema, item.Item2.Schema, StringComparison.OrdinalIgnoreCase);
                    var isChangeName = string.Equals(item.Item1.DbName, item.Item2.DbName, StringComparison.OrdinalIgnoreCase);
                    if (isChangeSchema || isChangeName)
                        yield return new RenameView
                        {
                            Schema = item.Item1.Schema,
                            ViewName = item.Item1.DbName,
                            NewSchema = isChangeSchema ? item.Item2.Schema : null,
                            NewViewName = isChangeName ? item.Item2.DbName : null
                        };

                    if (FullJoin(item.Item1.SqlBody, _ => _.Key, item.Item2.SqlBody, _ => _.Key).Any(i => i.Item1.Value != i.Item2.Value))
                    {
                        var av = new AlterView
                        {
                            Schema = item.Item2.Schema,
                            ViewName = item.Item2.DbName
                        };
                        var builder = new ViewBuilder(av);
                        foreach (var body in item.Item2.SqlBody)
                        {
                            builder.SetScript(body.Key, body.Value);
                        }
                        yield return av;
                    }
                }
            }
        }

        protected virtual IEnumerable<MigrationOperation> CompareFunctions(IEnumerable<IFunction> functions1, IEnumerable<IFunction> functions2)
        {
            //TODO: Добавить сравнение функций
            //var pairs = FullJoin(functions1, f => f.ClrName, functions2, f => f.ClrName);
            //foreach (var item in pairs)
            //{
            //    var current = item.Item1;
            //    var target = item.Item2;
            //    if (current == null)
            //        yield return new DropFunction
            //        {
            //            Schema = current.,
            //            FunctionName = current.
            //        };
            //    else
            //        yield return new CreateFunction
            //        {
                        
            //        }
            //}
            yield break;
        }
    }
}
