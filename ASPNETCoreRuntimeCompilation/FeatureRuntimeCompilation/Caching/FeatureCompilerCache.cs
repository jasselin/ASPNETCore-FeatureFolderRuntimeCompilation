using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public class FeatureCompilerCache : IFeatureCompilerCache
    {
        private readonly IDictionary<string, FeatureCompilerCacheResult> _cacheEntries;
        private readonly object _cacheLock = new object();

        private readonly IFeatureCompilerService _compilerService;

        public FeatureCompilerCache(IFeatureCompilerService compilerService)
        {
            _compilerService = compilerService;

            _cacheEntries = new Dictionary<string, FeatureCompilerCacheResult>();
        }

        public (FeatureCompilerCacheResult, bool NewAssembly) GetOrAdd(string cacheKey, string featurePath)
        {
            FeatureCompilerCacheResult cacheEntry;
            var newAssembly = false;

            lock (_cacheLock)
            {
                var isCached = _cacheEntries.TryGetValue(cacheKey, out cacheEntry);
                if (isCached && cacheEntry.IsExpired())
                {
                    //TODO: Does not seem to unload
                    cacheEntry.Result.AssemblyLoadContext.Unload();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    _cacheEntries.Remove(cacheKey);
                    cacheEntry = null;
                }

                if (cacheEntry == null)
                {
                    var compilerResult = _compilerService.Compile(featurePath);
                    if (!compilerResult.Success)
                        return (new FeatureCompilerCacheResult(compilerResult), false);

                    var fileProvider = new PhysicalFileProvider(featurePath)
                    {
                        /* https://github.com/aspnet/FileSystem/commit/b9986fc364ec439606f998d44e02be71a922aec7#diff-2155196e66e218ca71336e485e142309 
                            * Active polling par défaut dans .NET Core 2.2, ne semble pas détecter les modifications à toutes les sauvegarde des fichiers.
                            * On force le polling pour contourner le problème. */
                        UsePollingFileWatcher = true
                    };

                    var fileChangeToken = fileProvider.Watch("**/*.cs");
                    var expirationTokens = new List<IChangeToken>
                                        {
                                            fileChangeToken
                                        };

                    newAssembly = true;
                    cacheEntry = new FeatureCompilerCacheResult(compilerResult, expirationTokens);
                    _cacheEntries.Add(cacheKey, cacheEntry);
                }
            }

            return (cacheEntry, newAssembly);
        }
    }
}
