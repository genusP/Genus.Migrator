using System;
using Genus.Migrator.Model.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Genus.Migrator.Model.Builder
{
    public class ViewBuilder:DbObjectWithScriptsBuildercs<ViewBuilder>
    {

        protected override void Validate(string clrName) {
            base.Validate(clrName);
            if (Scripts.Count == 0)
                throw new InvalidOperationException($"Setup body script for view {clrName}");
        }

        protected internal virtual IView Build(string name, IModel model)
        {
            Validate(name);
            var view = new View(name, model);
            if (Name != null)
                view.DbName = Name;
            if (Schema != null)
                view.Schema = Schema;
            view.SqlBody = Scripts;
            view.With = Withs;
            return view;
        }
    }
}