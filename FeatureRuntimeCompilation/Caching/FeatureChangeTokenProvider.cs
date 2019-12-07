using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public class FeatureChangeTokenProvider : IFeatureChangeTokenProvider
    {
        private readonly ConcurrentDictionary<string, (CancellationTokenSource, IChangeToken)> _tokens = new ConcurrentDictionary<string, (CancellationTokenSource, IChangeToken)>();

        public IChangeToken GetToken(FeatureMetadata feature)
        {
            var tokenKey = feature.Name.ToLower();

            var (tokenSource, token) = _tokens.GetOrAdd(tokenKey, key =>
            {
                var tokenSource = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(tokenSource.Token);
                return (tokenSource, changeToken);
            });

            return token;
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
