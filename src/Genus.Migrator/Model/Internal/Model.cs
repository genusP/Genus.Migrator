using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    internal class Model : IModel
    {

        public IEnumerable<ITable> Tables
        {
            get;
            internal set;
        }

        public IEnumerable<IView> Views
        {
            get;
            internal set;
        }

        public IEnumerable<IFunction> Functions
        {
            get;
            internal set;
        }

        public ITable FindTable(string clrTableName)
        {
            return Tables.FirstOrDefault(_ => _.ClrName == clrTableName);
        }
    }
}
