using Microsoft.AspNetCore.Http;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IRuntimeFeatureProvider
    {
        RuntimeFeatureProviderResult GetFeature(HttpContext context);
        RuntimeFeatureProviderResult GetFeature(string featurePath);
    }
}