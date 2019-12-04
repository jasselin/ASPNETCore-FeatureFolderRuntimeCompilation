using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Matching;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureEndpointSelector : EndpointSelector
    {
        public override Task SelectAsync(HttpContext httpContext, CandidateSet candidates)
        {
            var candidate = candidates[candidates.Count - 1]; //TODO: get current assembly
            httpContext.SetEndpoint(candidate.Endpoint);
            httpContext.Request.RouteValues = candidate.Values;
            return Task.CompletedTask;
        }
    }
}
