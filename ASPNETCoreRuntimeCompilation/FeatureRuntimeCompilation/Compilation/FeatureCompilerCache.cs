namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerCache : IFeatureCompilerCache
    {
        private readonly IFeatureCompilerService _compilerService;

        public FeatureCompilerCache(IFeatureCompilerService compilerService)
        {
            _compilerService = compilerService;
        }

        public FeatureCompilerCacheResult GetOrAdd(string controllerTypeName, string featurePath)
        {
            //TODO: Implement caching
            var compilerResult = _compilerService.Compile(featurePath);
            if (!compilerResult.Success)
                return new FeatureCompilerCacheResult(compilerResult);

            var cacheEntry = new FeatureCompilerCacheResult(compilerResult/*, expirationTokens*/);

            return cacheEntry;
        }
    }
}
