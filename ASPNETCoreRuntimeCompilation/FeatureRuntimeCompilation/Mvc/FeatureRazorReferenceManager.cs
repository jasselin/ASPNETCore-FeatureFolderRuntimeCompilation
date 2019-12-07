using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    internal class FeatureRazorReferenceManager : RazorReferenceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FeatureRazorReferenceManager> _logger;
        private readonly FeatureRuntimeCompilationOptions _options;

        private MetadataReference _defaultReference;
        private IList<MetadataReference> _compilationReferences;

        public FeatureRazorReferenceManager(ApplicationPartManager partManager, IOptions<MvcRazorRuntimeCompilationOptions> razorOptions,
            FeatureRuntimeCompilationOptions options, IHttpContextAccessor httpContextAccessor, ILogger<FeatureRazorReferenceManager> logger)
            : base(partManager, razorOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _options = options;
        }

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
                    var featureAssembly = _httpContextAccessor.HttpContext.GetEndpoint().GetEndpointAssembly();
                    if (featureAssembly == _options.Assembly)
                    {
                        _logger.LogDebug($"Razor ref assembly: DEFAULT");
                        additionalReferences.Add(_defaultReference);
                    }
                    else
                    {
                        _logger.LogDebug($"Razor ref assembly: {featureAssembly.FullName}");
                        additionalReferences.Add(CreateMetadataReference(featureAssembly.Location));
                    }
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
