using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class ModelBuilder
    {
        private static MethodInfo tableGenericMethod 
            = typeof(ModelBuilder).GetTypeInfo().DeclaredMethods.Single(m => m.IsGenericMethod && m.Name == nameof(Table));
        private readonly Lazy<SortedDictionary<string, TableBuilder>> _tables 
            = new Lazy<SortedDictionary<string, TableBuilder>>();
        private readonly Lazy<SortedDictionary<string, ViewBuilder>> _views 
            = new Lazy<SortedDictionary<string, ViewBuilder>>();
        private readonly Lazy<SortedDictionary<string, FunctionBuilder>> _functions
            = new Lazy<SortedDictionary<string, FunctionBuilder>>();

        public ModelBuilder()
        {
        }

        public TableBuilder Table(string name, Action<TableBuilder> setupAction = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));

            var builder = AddBuilder(name, _tables, ()=>new TableBuilder(), _views, "view");
            if (setupAction != null)
                setupAction(builder);
            return builder;
        }

        public TableBuilder Table(Type type, Action<TableBuilder> setupAction = null)
        {
            return (TableBuilder)
                tableGenericMethod
                    .MakeGenericMethod(type)
                    .Invoke(this, new object[] { setupAction});
        }

        public TableBuilder<TEntity> Table<TEntity>(Action<TableBuilder<TEntity>> setupAction = null)
        {
            var name = typeof(TEntity).FullName;
            var builder = AddBuilder(
                name, 
                _tables, 
                ()=> new TableBuilder<TEntity>(), 
                _views, 
                "view"
                ) as TableBuilder<TEntity> ;
            if (builder == null)
                throw new InvalidOperationException("Table builder not typed");

            if (setupAction!=null)
                setupAction(builder);

            return builder;
        }

        public ViewBuilder View(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty", nameof(name));

            return AddBuilder(name, _views, ()=>new ViewBuilder(), _tables, "table");
        }

        public ViewBuilder View<TEntity>()
        {
            return View(typeof(TEntity).FullName);
        }

        private T1 AddBuilder<T1, T2>(
            string name,
            Lazy<SortedDictionary<string, T1>> target,
            Func<T1> builderFactory,
            Lazy<SortedDictionary<string, T2>> target2,
            string target2_name)
            where T1 : new()
        {
            T1 builder;
            if (target.Value.TryGetValue(name, out builder))
                return builder;
            if (target2!=null && target2.IsValueCreated && target2.Value.ContainsKey(name))
                throw new InvalidOperationException($"This object alredy registred as {target2_name}.");
            builder = builderFactory();
            target.Value.Add(name, builder);
            return builder;
        }

        public FunctionBuilder Function(string objName)
        {
            if (string.IsNullOrWhiteSpace(objName))
                throw new ArgumentException("Name is empty", nameof(objName));
            return AddBuilder<FunctionBuilder, object>(objName, _functions, ()=>new FunctionBuilder(), null, null);
        }

        public IModel Build()
        {
            var model = new Internal.Model();
            var builder2tablePair = _tables.IsValueCreated
                            ? _tables.Value.ToDictionary(tb=>tb.Value, tb => tb.Value.Build(tb.Key, model))
                            : Enumerable.Empty<KeyValuePair<TableBuilder, ITable>>();
            model.Tables = builder2tablePair.Select(_ => _.Value).ToArray();

            model.Views = _views.IsValueCreated
                            ? _views.Value.Select(vb => vb.Value.Build(vb.Key, model)).ToArray()
                            : Enumerable.Empty<IView>();

            model.Functions = _functions.IsValueCreated
                            ? _functions.Value.Select(f => f.Value.Build(f.Key, model)).ToArray()
                            : Enumerable.Empty<IFunction>();

            foreach (var tb2t in builder2tablePair)
                tb2t.Key.BuildAssociations(tb2t.Value, model);

            return model;
        }
    }
}
