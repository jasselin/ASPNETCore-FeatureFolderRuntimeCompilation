using Microsoft.AspNetCore.Http;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IRuntimeFeatureProvider
    {
        RuntimeFeatureProviderResult GetFeature(string featurePath);
    }
}