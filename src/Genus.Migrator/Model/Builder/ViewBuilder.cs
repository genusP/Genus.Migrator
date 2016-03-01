using System;
using Genus.Migrator.Model.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Genus.Migrator.Model.Builder
{
    public class ViewBuilder:DbObjectBuider<ViewBuilder>
    {
        readonly Dictionary<ProviderName, string> _bodyScripts = new Dictionary<ProviderName, string>();
        readonly Dictionary<ProviderName, string> _with = new Dictionary<ProviderName, string>();

        public ViewBuilder SetBodyScript(ProviderName provider, string selectScript)
        {
            if (_bodyScripts.ContainsKey(provider))
                _bodyScripts[provider] = selectScript;
            else
                _bodyScripts.Add(provider, selectScript);
            return this;
        }

        public ViewBuilder With(ProviderName provider, string with)
        {
            if (_with.ContainsKey(provider))
                _with[provider] = with;
            else
                _with.Add(provider, with);
            return this;
        }

        protected override void Validate(string clrName) {
            base.Validate(clrName);
            if (_bodyScripts.Count == 0)
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
            view.SqlBody = new ReadOnlyDictionary<ProviderName, string>(_bodyScripts);
            view.With = new ReadOnlyDictionary<ProviderName, string>(_with);
            return view;
        }
    }
}