using Microsoft.Extensions.Primitives;

namespace FeatureRuntimeCompilation.Caching
{
    internal interface IFeatureChangeTokenProvider
    {
        IChangeToken GetToken(FeatureMetadata feature);
        void CancelToken(FeatureMetadata feature);
    }
}
