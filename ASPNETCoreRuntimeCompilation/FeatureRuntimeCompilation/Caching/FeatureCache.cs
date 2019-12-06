using System;
using System.Collections.Concurrent;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public class FeatureCache : IFeatureCache
    {
        private readonly IFeatureCompilerService _compiler;

        private readonly ConcurrentDictionary<string, FeatureCompilerResult> _cachedResults = new ConcurrentDictionary<string, FeatureCompilerResult>();

        public FeatureCache(IFeatureCompilerService compiler)
        {
            _compiler = compiler;
        }

        public FeatureCompilerResult Get(FeatureMetadata feature)
        {
            var cacheKey = GetCacheKey(feature);
            _cachedResults.TryGetValue(cacheKey, out var cacheItem);
            return cacheItem;
        }

        public FeatureCompilerResult GetOrUpdate(FeatureMetadata feature)
        {
            var cacheKey = GetCacheKey(feature);
            _cachedResults.TryGetValue(cacheKey, out var cacheItem);

            if (IsUpToDate(cacheItem))
                return cacheItem;

            var newItem = _compiler.Compile(feature.Name, feature.FeaturePath);
            _cachedResults.AddOrUpdate(cacheKey, newItem, (key, oldValue) => newItem);

            return newItem;
        }

        private string GetCacheKey(FeatureMetadata feature) => feature.Name.ToLower();

        private bool IsUpToDate(FeatureCompilerResult cacheItem)
        {
            return false;
            //return cacheItem != null; //TODO: checksum
        }
    }
}
