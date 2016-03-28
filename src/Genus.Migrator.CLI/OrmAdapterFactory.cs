using Genus.Migrator.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Migrator.CLI
{
    public static class OrmAdapterFactory
    {
        public static IOrmAdapter Create(string adapterClassName, string environment, IServiceCollection services=null)
        {
            var assembly = Assembly.Load(new AssemblyName(PlatformServices.Default.Application.ApplicationName));
            var adapterType = string.IsNullOrWhiteSpace(adapterClassName)
                ? assembly.DefinedTypes.First(t =>
                         !t.IsAbstract
                         && !t.IsGenericTypeDefinition
                         && typeof(IOrmAdapter).IsAssignableFrom(t.AsType()))
                : assembly.GetType(adapterClassName).GetTypeInfo();
            var res = (IOrmAdapter)Activator.CreateInstance(adapterType.AsType());
            res.Services = services;
            res.StartupAssembly = assembly;
            res.Environment = environment;
            return res;
        }
    }
}
