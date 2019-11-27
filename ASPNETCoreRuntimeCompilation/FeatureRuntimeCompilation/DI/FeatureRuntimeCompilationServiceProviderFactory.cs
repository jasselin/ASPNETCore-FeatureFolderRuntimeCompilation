using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.DI
{
    public class FeatureRuntimeCompilationServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {   
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = new FeatureRuntimeCompilationServiceProvider();

            containerBuilder.AddSingleton<IFeatureRuntimeCompilationServiceProvider>(serviceProvider);

            serviceProvider.AddServices(containerBuilder);

            return serviceProvider;
        }
    }
}
