using Genus.Migrator.Model.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Migrator.Model.Builder
{
    public class AssociationBuilder
    {
        public AssociationBuilder()
        {

        }

        string ForeignKeyName { get; set; }

        ForeignKeyAction? OnDeleteAction { get; set; }

        ForeignKeyAction? OnUpdateAction { get; set; }

        string DependNavigation { get; set; }

        string PrincipalNavigation { get; set; }

        public AssociationBuilder WithNavigation(string dependNavigation, string principalNavigation)
        {
            DependNavigation = dependNavigation;
            PrincipalNavigation = principalNavigation;
            return this;
        }

        public AssociationBuilder AsForeignKey(string name)
        {
            ForeignKeyName = name;
            return this;
        }

        public AssociationBuilder OnDelete(ForeignKeyAction action)
        {
            OnDeleteAction = action;
            return this;
        }


        public AssociationBuilder OnUpdate(ForeignKeyAction action)
        {
            OnUpdateAction = action;
            return this;
        }


        internal IAssociation Build(IField dependField, IField principalField)
        {
            var association = new Association(dependField, principalField)
            {
                DependentNavigation = DependNavigation,
                ForeignKeyName = ForeignKeyName,
                PrincipalNavigation = PrincipalNavigation,
                OnDeleteAction = OnDeleteAction,
                OnUpdateAction = OnUpdateAction
            };
            return association;
        }
    }
}
