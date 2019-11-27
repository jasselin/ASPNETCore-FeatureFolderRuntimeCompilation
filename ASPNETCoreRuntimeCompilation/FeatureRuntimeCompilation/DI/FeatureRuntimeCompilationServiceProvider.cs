using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.DI
{
    public class FeatureRuntimeCompilationServiceProvider : IServiceProvider, IFeatureRuntimeCompilationServiceProvider
    {
        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        public FeatureRuntimeCompilationServiceProvider()
        {
            _serviceCollection = new ServiceCollection();
        }

        public void AddServices(IEnumerable<ServiceDescriptor> services)
        {
            foreach (var service in services)
                _serviceCollection.Add(service);

            //TODO: What to do with existing instances created by the previous container?
            //TODO: Dispose of the previous container?
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
