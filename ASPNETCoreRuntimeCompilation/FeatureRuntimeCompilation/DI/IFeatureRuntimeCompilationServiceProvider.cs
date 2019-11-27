using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.DI
{
    public interface IFeatureRuntimeCompilationServiceProvider
    {
        void AddServices(IEnumerable<ServiceDescriptor> services);
    }
}