using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    internal class IndexItem : IIndexItem
    {
        public bool Decending
        {
            get;
            set;
        }

        public IField Field
        {
            get;
            set;
        }
    }
}
