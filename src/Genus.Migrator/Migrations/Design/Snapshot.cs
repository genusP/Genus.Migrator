using Genus.Migrator.Model;
using Genus.Migrator.Model.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations
{
    public abstract class Snapshot
    {
        readonly Lazy<IModel> _model;

        public Snapshot()
        {
            _model = new Lazy<IModel>(() =>
            {
                var mb = new ModelBuilder();
                BuildModel(mb);
                return mb.Build();
            });
        }

        public IModel Model => _model.Value;

        protected abstract void BuildModel(ModelBuilder modelBuilder);
    }
}
