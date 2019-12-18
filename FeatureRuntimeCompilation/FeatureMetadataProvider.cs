using FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Linq;

namespace FeatureRuntimeCompilation
{
    internal class FeatureMetadataProvider : IFeatureMetadataProvider
    {
        private readonly FeatureRuntimeCompilationOptions _options;

        public FeatureMetadataProvider(FeatureRuntimeCompilationOptions options)
        {
            _options = options;
        }

        public FeatureMetadata GetMetadataFor(RouteValueDictionary routeValues)
        {
            if (routeValues == null || routeValues.Count == 0)
                return null;

            var requestValues = routeValues.Where(x => x.Key.StartsWith("level") || x.Key.Equals("controller")).Select(x => x.Value);

            var featurePath = Path.Combine(_options.FeaturesPath, string.Join("\\", requestValues));
            if (!Directory.Exists(featurePath))
                return null;

            var featureName = string.Concat(_options.FeatureNamespace, ".", string.Join('.', requestValues));
            var controllerTypeName = string.Concat(featureName, ".", requestValues.Last(), "Controller");

            return new FeatureMetadata(featureName, controllerTypeName, featurePath);
        }

        public FeatureMetadata GetMetadataFor(string featurePath)
        {
            var controllerName = new DirectoryInfo(featurePath).Name;
            var featureName = string.Concat(_options.FeatureNamespace, featurePath.Substring(_options.FeaturesPath.Length).Replace("\\", "."));
            var controllerTypeName = string.Concat(featureName, ".", controllerName, "Controller");

            return new FeatureMetadata(featureName, controllerTypeName, featurePath);
        }
    }
}