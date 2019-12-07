using FeatureRuntimeCompilation.Caching;
using FeatureRuntimeCompilation.Compilation;
using FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FeatureRuntimeCompilation.Mvc
{
    public class FeatureEndpointSelector : EndpointSelector
    {
        private readonly FeatureRuntimeCompilationOptions _options;
        private readonly IFeatureMetadataProvider _metadataProvider;
        private readonly IFeatureCache _featureCache;
        private readonly ILogger<FeatureEndpointSelector> _logger;

        public FeatureEndpointSelector(FeatureRuntimeCompilationOptions options,
            IFeatureMetadataProvider metadataProvider, IFeatureCache featureCache,
            ILogger<FeatureEndpointSelector> logger)
        {
            _options = options;
            _metadataProvider = metadataProvider;
            _featureCache = featureCache;
            _logger = logger;
        }

        public override Task SelectAsync(HttpContext httpContext, CandidateSet candidates)
        {
            if (candidates.Count == 0)
                return Task.CompletedTask;

            // Find the last built endpoint
            var candidate = candidates[0];

            var feature = _metadataProvider.GetMetadataFor(candidate.Values);
            var result = _featureCache.Get(feature);
            if (result != null)
            {
                if (!result.Success)
                    throw new FeatureCompilationFailedException(_options.ProjectPath, result);

                for (var i = candidates.Count - 1; i > 0; i--)
                {
                    var assembly = candidates[i].Endpoint.GetEndpointAssembly();
                    if (result.Assembly == assembly)
                    {
                        _logger.LogDebug($"Endpoint assembly: {assembly.FullName}");
                        candidate = candidates[i];
                        break;
                    }
                }
            }

            httpContext.SetEndpoint(candidate.Endpoint);
            httpContext.Request.RouteValues = candidate.Values;

            return Task.CompletedTask;
        }
    }
}
