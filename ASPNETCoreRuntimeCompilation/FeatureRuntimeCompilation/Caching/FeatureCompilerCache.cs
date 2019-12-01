using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching
{
    public class FeatureCompilerCache : IFeatureCompilerCache
    {
        private readonly IDictionary<string, FeatureCompilerCacheResult> _cacheEntries;
        private readonly object _cacheLock = new object();

        private readonly IFeatureCompilerService _compilerService;
        private readonly ApplicationPartManager _appPartManager;
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly EndpointDataSource _endpointDataSource;

        public FeatureCompilerCache(IFeatureCompilerService compilerService, ApplicationPartManager appPartManager, EndpointDataSource endpointDataSource, FeatureRuntimeCompilationActionDescriptorChangeProvider actionDescriptorChangeProvider)
        {
            _compilerService = compilerService;
            _appPartManager = appPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
            _endpointDataSource = endpointDataSource;
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
                    var part = _appPartManager.ApplicationParts.OfType<AssemblyPart>().SingleOrDefault(x => x.Assembly == cacheEntry.Result.Assembly);
                    _appPartManager.ApplicationParts.Remove(part);
                    
                    // Removes controller endpoint from cache
                    _actionDescriptorChangeProvider.TokenSource.Cancel();

                    var ct = _endpointDataSource.GetChangeToken();
                    //var pi = ct.GetType().GetProperty("Token", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    //var cancellationToken = (CancellationToken)pi.GetValue(ct);
                    //cancellationToken.

                    //var endpoints = _endpointDataSource.Endpoints.Where(x => x.DisplayName.Contains(cacheEntry.Result.Assembly.GetName().Name));
                    //foreach (var endpoint in endpoints)
                    //{
                    //    _endpointDataSource.Endpoints.r
                    //}

                    _cacheEntries.Remove(cacheKey);
                    var assemblyLoadContextRef = cacheEntry.Result.AssemblyLoadContextRef;
                    cacheEntry = null;

                    //TODO: Does not seem to unload
                    var assemblyLoadContext = assemblyLoadContextRef.Target as FeatureAssemblyLoadContext;
                    assemblyLoadContext.Unload();
                    for (var i = 0; assemblyLoadContextRef.IsAlive && (i < 10); i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    //if (assemblyLoadContextRef.IsAlive)
                    //    throw new Exception("Still alive");

                    //TODO: delete assembly file
                }

                if (cacheEntry == null)
                {
                    var compilerResult = _compilerService.Compile(cacheKey, featurePath);
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
