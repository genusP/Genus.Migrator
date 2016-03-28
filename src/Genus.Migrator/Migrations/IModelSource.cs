using Genus.Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations
{
    public interface IModelSource
    {
        IModel Model { get; }
    }
}
