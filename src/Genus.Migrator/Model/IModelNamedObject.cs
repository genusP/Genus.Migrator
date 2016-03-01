using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model
{
    public interface IModelNamedObject
    {
        string ClrName { get; }
    }
}
