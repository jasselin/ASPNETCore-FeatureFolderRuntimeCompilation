using Microsoft.Extensions.Primitives;

namespace FeatureRuntimeCompilation.Caching
{
    public interface IFeatureChangeTokenProvider
    {
        IChangeToken GetToken(FeatureMetadata feature);
        void CancelToken(FeatureMetadata feature);
    }
}
