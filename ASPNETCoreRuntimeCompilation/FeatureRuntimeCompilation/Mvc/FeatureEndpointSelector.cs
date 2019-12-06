using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Matching;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureEndpointSelector : EndpointSelector
    {
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
                    if (creationTime != null && (creationTime >= candidateCreationTime || candidateCreationTime == null))
                    {
                        candidate = candidates[i];
                        candidateCreationTime = creationTime;
                    }
                }
            }

            httpContext.SetEndpoint(candidate.Endpoint);
            httpContext.Request.RouteValues = candidate.Values;

            return Task.CompletedTask;
        }

        private DateTime? GetCandidateCreationTime(CandidateState candidate)
        {
            var assembly = candidate.Endpoint.GetEndpointAssembly();
            if (string.IsNullOrEmpty(assembly.Location))
                return null;

            return File.GetLastWriteTime(assembly.Location);
        }
    }
}
