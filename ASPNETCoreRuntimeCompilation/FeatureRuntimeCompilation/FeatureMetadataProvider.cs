using System.IO;
using System.Linq;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Http;

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
            var requestValues = context.Request.Path.ToString().Trim('/').Split('/');

            var assemblyName = _options.Assembly.GetName().Name;

            var controllerTypeName = string.Concat(assemblyName, ".Features.", string.Join('.', requestValues), ".", requestValues.Last(), "Controller");
            var featurePath = Path.Combine(_options.ProjectPath, "Features", string.Join("\\", requestValues));

            return new FeatureMetadata(controllerTypeName, featurePath);
        }
    }
}