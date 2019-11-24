using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public static class FeatureRuntimeCompilationExtensions
    {
        public static IMvcBuilder AddFeatureRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
            var services = mvcBuilder.Services;

            services.AddSingleton(options);

            //services.AddSingleton<IActionSelector>(serviceProvider =>
            //{
            //    var actionSelector = serviceProvider.GetRequiredService<IActionSelector>();
            //    return new FeatureRuntimeCompilationActionSelector(actionSelector);
            //});

            //services.AddSingleton<IActionSelector, FeatureRuntimeCompilationActionSelector>();

            // Add controllers to service provider to be resolved by FeatureRuntimeCompilationControllerActivator
            RegisterControllers(services, options);

            services.AddSingleton<IControllerActivator, FeatureRuntimeCompilationControllerActivator>();

            services.AddSingleton<IRuntimeFeatureProvider, RuntimeFeatureProvider>();
            services.AddSingleton<IFeatureCompilerCache, FeatureCompilerCache>();
            services.AddTransient<IFeatureCompilerService, FeatureCompilerService>();

            return mvcBuilder;
        }

        private static void RegisterControllers(IServiceCollection services, FeatureRuntimeCompilationOptions options)
        {
            foreach (var controllerType in options.Assembly.ExportedTypes.Where(x => typeof(Controller).IsAssignableFrom(x)))
                services.AddTransient(controllerType);
        }

        public static IApplicationBuilder UseFeatureRuntimeCompilation(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<FeatureRuntimeCompilationOptions>();

            RemoveAssemblyFromApplicationPartManager(app, options);

            return app;
        }

        private static void RemoveAssemblyFromApplicationPartManager(IApplicationBuilder app, FeatureRuntimeCompilationOptions options)
        {
            var appPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();

            var assemblyPart = appPartManager.ApplicationParts
                .Where(x => x is AssemblyPart)
                .Cast<AssemblyPart>()
                .FirstOrDefault(x => x.Assembly == options.Assembly);

            if (assemblyPart == null)
                throw new Exception($"Assembly '{options.Assembly.FullName}' is not loaded by the application.");

            // TODO: If removed, we loose route values. Still need to remove?
            //appPartManager.ApplicationParts.Remove(assemblyPart);
        }
    }
}
