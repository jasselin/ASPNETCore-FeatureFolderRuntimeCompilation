using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing.Matching;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureEndpointSelector : EndpointSelector
    {
        private readonly IFeatureMetadataProvider _featureMetadataProvider;

        public FeatureEndpointSelector(IFeatureMetadataProvider featureMetadataProvider)
        {
            _featureMetadataProvider = featureMetadataProvider;
        }

        public override Task SelectAsync(HttpContext httpContext, CandidateSet candidates)
        {
            if (candidates.Count == 0)
                return Task.CompletedTask;

            // Find the last built endpoint
            var candidate = candidates[0];

            if (candidates.Count > 1)
            {
                var candidateCreationTime = GetCandidateCreationTime(candidate);
                for (var i = 1; i < candidates.Count; i++)
                {
                    var creationTime = GetCandidateCreationTime(candidates[i]);
                    if (creationTime >= candidateCreationTime)
                    {
                        candidate = candidates[i];
                        candidateCreationTime = creationTime;
                    }
                }
            }

            httpContext.SetEndpoint(candidate.Endpoint);
            httpContext.Request.RouteValues = candidate.Values;

            //TODO: static property
            httpContext.Items["FeatureMetadata"] = _featureMetadataProvider.GetMetadataFor(candidate.Values);

            return Task.CompletedTask;
        }

        private DateTime GetCandidateCreationTime(CandidateState candidate)
        {
            return candidate.Endpoint.Metadata
                .OfType<ControllerActionDescriptor>()
                .Select(x => x.ControllerTypeInfo.Assembly)
                .Where(x => !string.IsNullOrEmpty(x.Location))
                .Select(x => File.GetLastWriteTime(x.Location))
                .Single();
        }
    }
}
