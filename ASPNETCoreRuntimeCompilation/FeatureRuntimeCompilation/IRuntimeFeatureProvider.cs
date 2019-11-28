using Microsoft.AspNetCore.Http;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IRuntimeFeatureProvider
    {
        //RuntimeFeatureProviderResult GetFeature(RouteValueDictionary routeValues);
        RuntimeFeatureProviderResult GetFeature(HttpContext context);

        //[Obsolete]
        //Type GetControllerType(ControllerContext context);
    }
}