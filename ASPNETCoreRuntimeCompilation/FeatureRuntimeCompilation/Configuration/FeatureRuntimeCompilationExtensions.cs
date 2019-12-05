using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public static class FeatureRuntimeCompilationExtensions
    {
        public static IMvcBuilder AddFeatureRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
            var services = mvcBuilder.Services;

            //services.AddSingleton<IFeatureRuntimeCompilationServiceProvider, FeatureRuntimeCompilationServiceProvider>();

            // ASP.NET (Http, Routing, Razor)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<EndpointSelector, FeatureEndpointSelector>();
            services.AddSingleton<RazorReferenceManager, FeatureRazorReferenceManager>();
            services.AddSingleton<FeatureRuntimeCompilationActionDescriptorChangeProvider>();
            services.AddSingleton<IActionDescriptorChangeProvider>(sp => sp.GetService<FeatureRuntimeCompilationActionDescriptorChangeProvider>());

            mvcBuilder.AddRazorRuntimeCompilation(options);

            // Compilation
            services.AddSingleton(options);
            services.AddSingleton<RuntimeFeatureCompilationWatcher>();
            services.AddTransient<IFeatureMetadataProvider, FeatureMetadataProvider>();
            services.AddSingleton<IRuntimeFeatureProvider, RuntimeFeatureProvider>();
            services.AddSingleton<IFeatureCompilerService, FeatureCompilerService>();
            services.AddSingleton<IFeatureCompilerCache, FeatureCompilerCache>();

            // Setup
            if (Directory.Exists(options.AssembliesOutputPath))
                Directory.Delete(options.AssembliesOutputPath, true);

            Directory.CreateDirectory(options.AssembliesOutputPath);

            FeatureAssemblyLocator.Init();

            return mvcBuilder;
        }

        private static void AddRazorRuntimeCompilation(this IMvcBuilder mvcBuilder, FeatureRuntimeCompilationOptions options)
        {
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
        }

        public static IApplicationBuilder UseFeatureRuntimeCompilation(this IApplicationBuilder app)
        {
            var watcher = app.ApplicationServices.GetRequiredService<RuntimeFeatureCompilationWatcher>();
            var options = app.ApplicationServices.GetRequiredService<FeatureRuntimeCompilationOptions>();
            Task.Run(() => watcher.Watch(options));

            app.UseMiddleware<FeatureRuntimeCompilationMiddleware>();

            return app;
        }
    }
}
