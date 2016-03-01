using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Internal
{
    internal class Association : IAssociation
    {
        public Association(IField field, IField referenceField)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (referenceField == null)
                throw new ArgumentNullException(nameof(referenceField));
            Field = field;
            ReferenceField = referenceField;
        }

        public string ForeignKeyName
        {
            get;
            set;
        }

        public string DependentNavigation
        {
            get;
            set;
        }

        public IField Field
        {
            get;
        }

        public ForeignKeyAction? OnDeleteAction
        {
            get;
            set;
        }

        public ForeignKeyAction? OnUpdateAction
        {
            get;
            set;
        }

        public string PrincipalNavigation
        {
            get;
            set;
        }

        public IField ReferenceField
        {
            get;
        }
    }
}
