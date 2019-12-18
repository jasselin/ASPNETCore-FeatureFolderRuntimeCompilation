using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace FeatureRuntimeCompilation.Caching
{
    internal class FeatureChangeTokenProvider : IFeatureChangeTokenProvider
    {
        private readonly ConcurrentDictionary<string, (CancellationTokenSource, IChangeToken)> _tokens = new ConcurrentDictionary<string, (CancellationTokenSource, IChangeToken)>();

        public (CancellationTokenSource, IChangeToken) GetToken(FeatureMetadata feature)
        {
            var tokenKey = feature.Name.ToLower();

            return _tokens.GetOrAdd(tokenKey, key =>
            {
                var tokenSource = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(tokenSource.Token);
                return (tokenSource, changeToken);
            });
        }

        public void CancelToken(FeatureMetadata feature)
        {
            var tokenKey = feature.Name.ToLower();

            if (_tokens.TryRemove(tokenKey, out var tokenValue))
            {
                tokenValue.Item1.Cancel();
            }
        }
    }
}
