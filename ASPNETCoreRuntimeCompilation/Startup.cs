using ASPNETCoreRuntimeCompilation.Features.FeatureA;
using FeatureRuntimeCompilation.Configuration;
using FeatureRuntimeCompilation.FeatureFolders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASPNETCoreRuntimeCompilation
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .AddFeatureFolders()
                .AddFeatureRuntimeCompilation(new FeatureRuntimeCompilationOptions(
                    typeof(Startup).Assembly, // Assembly to be dynamically compiled at runtime
                    _env.ContentRootPath));

            services.AddTransient<IFeatureADependency, FeatureADependency>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseFeatureRuntimeCompilation();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); // Attribute routing
            });
        }
    }
}