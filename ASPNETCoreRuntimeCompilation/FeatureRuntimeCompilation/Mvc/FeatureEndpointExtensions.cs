using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public static class FeatureEndpointExtensions
    {
        public static Assembly GetEndpointAssembly(this Endpoint endpoint)
        {
            return endpoint.Metadata
                .OfType<ControllerActionDescriptor>()
                .Select(x => x.ControllerTypeInfo.Assembly)
                .Single();
        }
    }
}
