using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public static class FeatureRuntimeCompilationExtensions
    {
        public static IMvcBuilder AddFeatureRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
            var services = mvcBuilder.Services;

            services.AddSingleton(options);

            // Add controllers to service provider to be resolved by FeatureRuntimeCompilationControllerActivator
            //RegisterControllers(services, options);

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSingleton<IFeatureRuntimeCompilationServiceProvider, FeatureRuntimeCompilationServiceProvider>();

            //services.AddSingleton<IControllerActivator, FeatureRuntimeCompilationControllerActivator>();
            services.AddSingleton<FeatureRuntimeCompilationActionDescriptorChangeProvider>();
            services.AddSingleton<IActionDescriptorChangeProvider>(sp => sp.GetService<FeatureRuntimeCompilationActionDescriptorChangeProvider>());

            services.AddSingleton<IRuntimeFeatureProvider, RuntimeFeatureProvider>();
            services.AddTransient<IFeatureMetadataProvider, FeatureMetadataProvider>();
            services.AddSingleton<IFeatureCompilerCache, FeatureCompilerCache>();
            services.AddTransient<IFeatureCompilerService, FeatureCompilerService>();

            if (Directory.Exists(options.AssembliesOutputPath))
                Directory.Delete(options.AssembliesOutputPath, true);

            Directory.CreateDirectory(options.AssembliesOutputPath);

            mvcBuilder.AddRazorRuntimeCompilation(opts =>
            {
                opts.FileProviders.Clear();
                opts.FileProviders.Add(new FeatureRuntimeCompilationPhysicalFileProvider(options.ProjectPath));

                // References are missing because we remove the main assembly application part
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x != options.Assembly))
                    opts.AdditionalReferencePaths.Add(assembly.Location);

                opts.AdditionalReferencePaths.Add(typeof(IHtmlContent).Assembly.Location);
                opts.AdditionalReferencePaths.Add(typeof(RazorCompiledItem).Assembly.Location);
            });

            FeatureAssemblyLocator.Init();

            return mvcBuilder;
        }

        //private static void RegisterControllers(IServiceCollection services, FeatureRuntimeCompilationOptions options)
        //{
        //    foreach (var controllerType in options.Assembly.ExportedTypes.Where(x => typeof(Controller).IsAssignableFrom(x)))
        //        services.AddTransient(controllerType);
        //}

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

            //TODO: don't include controllerfeature from the start
            appPartManager.ApplicationParts.Remove(assemblyPart);
        }
    }
}
