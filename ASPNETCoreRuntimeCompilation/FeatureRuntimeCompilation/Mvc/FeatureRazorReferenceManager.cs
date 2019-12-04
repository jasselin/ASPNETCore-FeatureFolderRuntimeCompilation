using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    internal class FeatureRazorReferenceManager : RazorReferenceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeatureCompilerCache _compilerCache;
        private readonly FeatureRuntimeCompilationOptions _options;

        public FeatureRazorReferenceManager(ApplicationPartManager partManager, IOptions<MvcRazorRuntimeCompilationOptions> razorOptions,
            FeatureRuntimeCompilationOptions options, IHttpContextAccessor httpContextAccessor, IFeatureCompilerCache compilerCache)
            : base(partManager, razorOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _compilerCache = compilerCache;
            _options = options;
        }

        private MetadataReference _defaultReference;
        private IList<MetadataReference> _compilationReferences;

        public override IReadOnlyList<MetadataReference> CompilationReferences
        {
            get
            {
                if (_compilationReferences == null)
                {
                    _defaultReference = base.CompilationReferences
                        .SingleOrDefault(x => x.Display.EndsWith(string.Concat(_options.AssemblyName, ".dll"), StringComparison.InvariantCultureIgnoreCase));

                    // Removes default project assembly
                    _compilationReferences = base.CompilationReferences
                        .Except(new[] { _defaultReference })
                        .ToList();
                }

                //TODO: meh...
                var additionalReferences = new List<MetadataReference>();
                if (_httpContextAccessor.HttpContext != null)
                {
                    var featureMetadata = _httpContextAccessor.HttpContext.GetFeatureMetadata();

                    var (feature, newAssembly) = _compilerCache.Get(featureMetadata.CacheKey);
                    var featureAssembly = feature?.Result?.Assembly;
                    if (featureAssembly != null)
                        additionalReferences.Add(CreateMetadataReference(featureAssembly.Location));
                }

                var references = _compilationReferences
                    .Union(additionalReferences)
                    .ToList();

                return references;
            }
        }

        // Copied from RazorReferenceManager
        private static MetadataReference CreateMetadataReference(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

                return assemblyMetadata.GetReference(filePath: path);
            }
        }
    }
}
