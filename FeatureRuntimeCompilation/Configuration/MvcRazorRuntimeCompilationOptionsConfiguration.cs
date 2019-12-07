using FeatureRuntimeCompilation.Caching;
using FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace FeatureRuntimeCompilation.Configuration
{
    internal class MvcRazorRuntimeCompilationOptionsConfiguration : IConfigureOptions<MvcRazorRuntimeCompilationOptions>
    {
        private readonly IFeatureMetadataProvider _metadataProvider;
        private readonly IFeatureChangeTokenProvider _tokenProvider;
        private readonly FeatureRuntimeCompilationOptions _options;

        public MvcRazorRuntimeCompilationOptionsConfiguration(IFeatureMetadataProvider metadataProvider,
            IFeatureChangeTokenProvider tokenProvider, FeatureRuntimeCompilationOptions options)
        {
            _metadataProvider = metadataProvider;
            _tokenProvider = tokenProvider;
            _options = options;
        }

        public void Configure(MvcRazorRuntimeCompilationOptions opts)
        {
            opts.FileProviders.Add(new FeatureFileProvider(_metadataProvider, _tokenProvider, _options));

            // References are missing because we remove the main assembly application part
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x != _options.Assembly))
                opts.AdditionalReferencePaths.Add(assembly.Location);

            opts.AdditionalReferencePaths.Add(typeof(IHtmlContent).Assembly.Location);
            opts.AdditionalReferencePaths.Add(typeof(RazorCompiledItem).Assembly.Location);
        }
    }
}
