using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IRuntimeFeatureProvider
    {
        RuntimeFeatureProviderResult GetFeature(RouteValueDictionary routeValues);

        //[Obsolete]
        //Type GetControllerType(ControllerContext context);
    }
}