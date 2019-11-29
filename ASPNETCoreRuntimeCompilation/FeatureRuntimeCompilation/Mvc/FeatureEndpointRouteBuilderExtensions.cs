using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public static class FeatureEndpointRouteBuilderExtensions
    {
        //TODO: Remove, use RouteAttribute through ControllerModelConvention
        public static IEndpointRouteBuilder MapFeatureControllers(this IEndpointRouteBuilder endpoints)
        {
            var options = endpoints.ServiceProvider.GetService<FeatureRuntimeCompilationOptions>();
            if (options == null)
                throw new Exception($"IServiceCollection.AddFeatureRuntimeCompilation must be called before {nameof(FeatureEndpointRouteBuilderExtensions)}.{nameof(MapFeatureControllers)}.");

            var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<IEndpointRouteBuilder>>();

            var assemblyNameLength = options.Assembly.GetName().Name.Length;
            var controllers = GetControllers(options.Assembly);
            foreach (var controller in controllers)
            {
                var segments = controller.FullName.Substring(assemblyNameLength + ".Features.".Length).Split('.');
                var defaults = new ExpandoObject();
                var routeValues = (IDictionary<string, object>)defaults;
                for (var i = 0; i < segments.Length - 1; i++)
                    routeValues.Add(i < segments.Length - 2 ? string.Concat("level", i + 1) : "controller", segments[i]);

                var url = string.Join('/', routeValues.Values);
                var pattern = string.Concat(url, "/{action=Index}");

                logger.LogInformation($"Mapping controller route for '{controller.FullName}' to '{pattern}'.");
                Console.WriteLine($"Mapping controller route for '{controller.FullName}' to '{pattern}'.");

                endpoints.MapControllerRoute(url, pattern, defaults);
            }

            return endpoints;
        }

        private static IList<Type> GetControllers(Assembly assembly)
        {
            return assembly.ExportedTypes
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(Controller).IsAssignableFrom(x))
                .OrderBy(x => x.FullName)
                .ToList();
        }
    }
}
