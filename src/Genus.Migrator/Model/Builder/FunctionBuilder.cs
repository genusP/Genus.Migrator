using System;
using Genus.Migrator.Model.Internal;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Genus.Migrator.Model.Builder
{
    public class FunctionBuilder
    {
        protected readonly Dictionary<ProviderName, string> _scripts = new Dictionary<ProviderName, string>();

        public FunctionBuilder SetScript(ProviderName providerName, string sqlScript)
        {
            if (_scripts.ContainsKey(providerName))
                _scripts[providerName] = sqlScript;
            else
                _scripts.Add(providerName, sqlScript);
            return this;
        }

        protected internal virtual IFunction Build(string name, IModel model)
        {
            if (_scripts.Count == 0)
                throw new InvalidOperationException($"Need setup sql script for function {name}");
            var result = new Function(name, model);
            result.Scripts = new ReadOnlyDictionary<ProviderName, string>(_scripts);
            return result;
        }
    }
}