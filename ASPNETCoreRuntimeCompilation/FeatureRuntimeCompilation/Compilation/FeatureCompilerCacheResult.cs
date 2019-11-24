using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerCacheResult
    {
        private readonly IEnumerable<IChangeToken> _expirationTokens;

        public FeatureCompilerCacheResult(FeatureCompilerResult result)
        {
            Result = result;
        }

        public FeatureCompilerCacheResult(FeatureCompilerResult result, IEnumerable<IChangeToken> expirationTokens)
            : this(result)
        {
            _expirationTokens = expirationTokens;
        }

        public FeatureCompilerResult Result { get; }

        public bool IsExpired()
        {
            return _expirationTokens.Any(token => token.HasChanged);
        }
    }
}
