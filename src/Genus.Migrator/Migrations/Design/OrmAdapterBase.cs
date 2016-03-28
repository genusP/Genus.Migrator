using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Genus.Migrator.DependecyInjection;
using Genus.Migrator.Model;
using System.Reflection;

namespace Genus.Migrator.Migrations.Design
{
    public abstract class OrmAdapterBase : IOrmAdapter
    {
        private readonly Lazy<IServiceProvider> _serviceProvider;

        public OrmAdapterBase()
        {
            _serviceProvider = new Lazy<IServiceProvider>(BuildServiceProvider);
        }

        public Assembly StartupAssembly { get; set; }

        public string Environment { get; set; }

        public IServiceCollection Services { get; set; }

        public IServiceProvider ServiceProvider
            => _serviceProvider.Value;

        public abstract ProviderName CurrentProvider { get; }

        private IServiceProvider BuildServiceProvider()
        {
            var services = Services??new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesFromStartup(services);
            services.AddSingleton<IOrmAdapter>(this);
            services.AddMigrator();
        }

        public abstract int ExecuteSql(string sqlText);

        protected virtual void ConfigureServicesFromStartup(IServiceCollection services)
        {
            if (StartupAssembly != null)
            {
                var startupClassNames = new[]
                    {
                    $"Startup_{Environment}",
                    "Startup",
                    "Program"
                    };
                var candidateTypes = StartupAssembly.DefinedTypes.Where(t => startupClassNames.Contains(t.Name, StringComparer.OrdinalIgnoreCase));
                var startupType = candidateTypes.OrderByDescending(_ => _.Name).FirstOrDefault();
                if (startupType != null)
                {

                    var methodNames = new[]
                    {
                    $"Configure{Environment}Services",
                    "ConfigureServices"
                    };

                    var methodInfo = methodNames.Select(n => startupType.GetDeclaredMethod(n)).FirstOrDefault(_ => _ != null);
                    if (methodInfo != null)
                    {
                        object instance = methodInfo.IsStatic 
                            ? null 
                            : ActivatorUtilities.GetServiceOrCreateInstance(ConfigureAppServices(Environment).BuildServiceProvider(), startupType.AsType());
                        methodInfo.Invoke(instance, new object[] { services });
                    }
                }
            }
        }

        protected virtual IServiceCollection ConfigureAppServices(string environment)
        {
            var services = new ServiceCollection();
            services.AddOptions();
            return services;
        }
    }
}
