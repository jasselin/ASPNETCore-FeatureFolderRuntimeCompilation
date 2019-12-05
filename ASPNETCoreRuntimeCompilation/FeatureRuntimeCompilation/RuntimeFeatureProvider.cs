﻿using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using System;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureProvider : IRuntimeFeatureProvider
    {
        private readonly FeatureRuntimeCompilationOptions _options;
        private readonly IFeatureCompilerCache _compilerCache;
        private readonly IFeatureMetadataProvider _featureMetadataProvider;

        public RuntimeFeatureProvider(FeatureRuntimeCompilationOptions options, IFeatureCompilerCache compilerCache, IFeatureMetadataProvider featureMetadataProvider)
        {
            _options = options;
            _compilerCache = compilerCache;
            _featureMetadataProvider = featureMetadataProvider;
        }

        public RuntimeFeatureProviderResult GetFeature(string featurePath)
        {
            var metadata = _featureMetadataProvider.GetMetadataFor(featurePath);
            if (metadata == null)
                return null;

            var (cacheResult, newAssembly) = _compilerCache.GetOrAdd(metadata.CacheKey, metadata.FeaturePath);

            var compilerResult = cacheResult.Result;
            if (!compilerResult.Success)
                throw new FeatureCompilationFailedException(_options.ProjectPath, compilerResult);

            var controllerType = compilerResult.Types.SingleOrDefault(x => x.FullName.Equals(metadata.ControllerTypeName, StringComparison.InvariantCultureIgnoreCase));

            return new RuntimeFeatureProviderResult(compilerResult.Assembly, newAssembly, controllerType);
        }
    }
}