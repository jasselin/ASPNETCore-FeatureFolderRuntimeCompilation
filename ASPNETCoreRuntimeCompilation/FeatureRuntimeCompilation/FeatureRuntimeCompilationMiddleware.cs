using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class FeatureRuntimeCompilationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FeatureRuntimeCompilationMiddlewareOptions _options;

        public FeatureRuntimeCompilationMiddleware(RequestDelegate next, FeatureRuntimeCompilationMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var routeData = context.GetRouteData();
            await _next(context);
        }
    }
}
