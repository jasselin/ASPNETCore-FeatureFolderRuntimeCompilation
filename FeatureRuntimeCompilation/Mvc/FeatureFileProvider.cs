using System;
using System.IO;
using System.Linq;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    internal class FeatureFileProvider : IFileProvider
    {
        private readonly IFeatureMetadataProvider _metadataProvider;
        private readonly IFeatureChangeTokenProvider _tokenProvider;
        private readonly FeatureRuntimeCompilationOptions _options;
        private readonly PhysicalFileProvider _innerProvider;

        public FeatureFileProvider(IFeatureMetadataProvider metadataProvider, 
             IFeatureChangeTokenProvider tokenProvider, FeatureRuntimeCompilationOptions options)
        {
            _metadataProvider = metadataProvider;
            _tokenProvider = tokenProvider;
            _options = options;

            _innerProvider = new PhysicalFileProvider(options.ProjectPath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) => _innerProvider.GetDirectoryContents(subpath);

        public IFileInfo GetFileInfo(string subpath) => _innerProvider.GetFileInfo(subpath);

        public IChangeToken Watch(string filter)
        {
            var fileInfo = _innerProvider.GetFileInfo(filter);

            if (fileInfo.Exists && !fileInfo.Name.StartsWith("_"))
            {
                var featurePath = GetFeaturePath(fileInfo.PhysicalPath);
                var feature = _metadataProvider.GetMetadataFor(featurePath);
                var featureChangeToken = _tokenProvider.GetToken(feature);

                var featureRelativePath = featurePath.Substring(_innerProvider.Root.Length).Replace("\\", "/");
                var fileChangeToken = _innerProvider.Watch(featureRelativePath + "/**/*");

                return new CompositeChangeToken(new[]
                {
                    fileChangeToken,
                    featureChangeToken
                });
            }

            return _innerProvider.Watch(filter);
        }

        private string GetFeaturePath(string filePath)
        {
            var currentPath = filePath;
            do
            {
                currentPath = Path.GetFullPath(Path.Combine(currentPath, ".."));
            }
            while (Directory.GetFiles(Path.Combine(currentPath, ".."), "*.cshtml").Any() && Path.GetFullPath(Path.Combine(currentPath, "..")) != _options.FeaturesPath);
            return currentPath;
        }
    }
}