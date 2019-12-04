using Microsoft.AspNetCore.Http;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public static class FeatureHttpContextExtensions
    {
        private static readonly string FeatureMetadataKey = "FeatureRuntimeCompilation:FeatureMetadata";

        public static FeatureMetadata GetFeatureMetadata(this HttpContext httpContext)
        {
            return httpContext.Items.ContainsKey(FeatureMetadataKey) ? httpContext.Items[FeatureMetadataKey] as FeatureMetadata : null;
        }

        public static void SetFeatureMetadata(this HttpContext httpContext, FeatureMetadata metadata)
        {
            httpContext.Items[FeatureMetadataKey] = metadata;
        }
    }
}
