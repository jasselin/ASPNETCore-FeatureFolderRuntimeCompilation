using System.Collections.Concurrent;
using System.Diagnostics;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using Microsoft.Extensions.Logging;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public class FeatureCache : IFeatureCache
    {
        private readonly IFeatureCompiler _compiler;
        private readonly IFeatureChecksumGenerator _checksumGenerator;
        private readonly ILogger<FeatureCache> _logger;
        private readonly ConcurrentDictionary<string, FeatureCompilerResult> _cachedResults = new ConcurrentDictionary<string, FeatureCompilerResult>();

        public FeatureCache(IFeatureCompiler compiler, IFeatureChecksumGenerator checksumGenerator,
            ILogger<FeatureCache> logger)
        {
            _compiler = compiler;
            _checksumGenerator = checksumGenerator;
            _logger = logger;
        }

        public FeatureCompilerResult Get(FeatureMetadata feature)
        {
            var cacheKey = GetCacheKey(feature);
            _cachedResults.TryGetValue(cacheKey, out var cacheItem);
            return cacheItem;
        }

        public (FeatureCompilerResult, bool hasUpdated) GetOrUpdate(FeatureMetadata feature)
        {
            var cacheKey = GetCacheKey(feature);
            _cachedResults.TryGetValue(cacheKey, out var cacheItem);

            if (cacheItem != null && IsUpToDate(feature, cacheItem))
                return (cacheItem, false);

            var checksum = _checksumGenerator.GetChecksum(feature);

            var sw = Stopwatch.StartNew();
            var newItem = _compiler.Compile(feature.Name, feature.FeaturePath, checksum);
            sw.Stop();
            _logger.LogInformation($"Feature '{feature.Name}' compiled in {sw.ElapsedMilliseconds}ms.");

            _cachedResults.AddOrUpdate(cacheKey, newItem, (key, oldValue) => newItem);

            return (newItem, true);
        }

        private string GetCacheKey(FeatureMetadata feature) => feature.Name.ToLower();

        private bool IsUpToDate(FeatureMetadata feature, FeatureCompilerResult cacheItem)
        {
            var checksum = _checksumGenerator.GetChecksum(feature);
            return checksum.Equals(cacheItem.Checksum);
        }
    }
}
