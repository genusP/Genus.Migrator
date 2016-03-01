using Genus.Migrator.Model.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Conventions
{
    public interface ITableConvention
    {
        void Apply<TEntity>(TableBuilder<TEntity> tableBuilder, ModelBuilder modelBuilder);
    }
}
