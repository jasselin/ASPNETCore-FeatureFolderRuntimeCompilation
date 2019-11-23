using Microsoft.AspNetCore.Builder;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public static class FeatureRuntimeCompilationMiddlewareExtensions
    {
        public static IApplicationBuilder UseFeatureRuntimeCompilation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<FeatureRuntimeCompilationMiddleware>();
        }
    }
}
