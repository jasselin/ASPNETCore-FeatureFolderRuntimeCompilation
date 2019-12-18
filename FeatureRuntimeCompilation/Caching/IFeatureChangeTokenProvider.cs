using Microsoft.Extensions.Primitives;
using System.Threading;

namespace FeatureRuntimeCompilation.Caching
{
    internal interface IFeatureChangeTokenProvider
    {
        (CancellationTokenSource, IChangeToken) GetToken(FeatureMetadata feature);
        void CancelToken(FeatureMetadata feature);
    }
}
