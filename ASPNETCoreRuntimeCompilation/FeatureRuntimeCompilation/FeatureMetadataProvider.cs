using System.IO;
using System.Linq;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureMetadataProvider : IFeatureMetadataProvider
    {
        private readonly FeatureRuntimeCompilationOptions _options;

        public FeatureMetadataProvider(FeatureRuntimeCompilationOptions options)
        {
            _options = options;
        }

        public FeatureMetadata GetMetadataFor(HttpContext context)
        {
            var routeData = context.GetRouteData();
            var requestValues = routeData.Values.Where(x => x.Key != "action" && x.Key != "controller").Select(x => x.Value);

            var featurePath = Path.Combine(_options.ProjectPath, "Features", string.Join("\\", requestValues));
            if (!Directory.Exists(featurePath))
                return null;

            var featureName = string.Concat(_options.AssemblyName, ".Features.", string.Join('.', requestValues));
            var controllerTypeName = string.Concat(featureName, ".", requestValues.Last(), "Controller");

            return new FeatureMetadata(featureName, controllerTypeName, featurePath);
        }

        public FeatureMetadata GetMetadataFor(string featurePath)
        {
            var featureName = new DirectoryInfo(featurePath).Name;
            var controllerTypeName = string.Concat(_options.FeatureNamespace, ".", featureName, ".", featureName, "Controller"); // TODO: fix

            return new FeatureMetadata(featureName, controllerTypeName, featurePath);
        }
    }
}