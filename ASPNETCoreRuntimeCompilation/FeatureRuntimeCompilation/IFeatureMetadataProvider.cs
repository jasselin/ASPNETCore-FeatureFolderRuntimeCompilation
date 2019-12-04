using Microsoft.AspNetCore.Http;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IFeatureMetadataProvider
    {
        FeatureMetadata GetMetadataFor(HttpContext context);
        FeatureMetadata GetMetadataFor(string featurePath);
    }
}