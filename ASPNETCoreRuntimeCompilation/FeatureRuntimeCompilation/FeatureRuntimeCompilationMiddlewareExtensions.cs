using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public static class FeatureRuntimeCompilationMiddlewareExtensions
    {
        public static IApplicationBuilder UseFeatureRuntimeCompilation(this IApplicationBuilder app, FeatureRuntimeCompilationMiddlewareOptions options)
        {
            RemoveAssemblyFromApplicationPartManager(app, options);

            return app.UseMiddleware<FeatureRuntimeCompilationMiddleware>(options);
        }

        private static void RemoveAssemblyFromApplicationPartManager(IApplicationBuilder app, FeatureRuntimeCompilationMiddlewareOptions options)
        {
            var appPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            var assemblyPart = appPartManager.ApplicationParts
                .Where(x => x is AssemblyPart)
                .Cast<AssemblyPart>()
                .FirstOrDefault(x => x.Assembly == options.Assembly);

            if (assemblyPart == null)
                throw new Exception($"Assembly '{options.Assembly.FullName}' is not loaded by the application.");

            appPartManager.ApplicationParts.Remove(assemblyPart);
        }
    }
}
