using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model
{
    public interface IModel
    {
        IEnumerable<ITable> Tables { get; }
        IEnumerable<IView> Views { get; }
        //IEnumerable<IIndex> Indexes { get; }
        //IEnumerable<ITrigger> Triggers { get; }
        //IEnumerable<IStoredProc> StoredProcedures { get; }
        IEnumerable<IFunction> Functions { get; }

        ITable FindTable(string clrTableName);
    }
}
