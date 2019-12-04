using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeatureRuntimeCompilationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IRuntimeFeatureProvider _featureProvider;
        private readonly ApplicationPartManager _appPartManager;
        private readonly FeatureRuntimeCompilationActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly ILogger<FeatureRuntimeCompilationMiddleware> _logger;

        public FeatureRuntimeCompilationMiddleware(RequestDelegate next, ILogger<FeatureRuntimeCompilationMiddleware> logger,
                                                   IRuntimeFeatureProvider featureProvider,
                                                   ApplicationPartManager appPartManager,
                                                   FeatureRuntimeCompilationActionDescriptorChangeProvider actionDescriptorChangeProvider)
        {
            _next = next;
            _logger = logger;
            _featureProvider = featureProvider;
            _appPartManager = appPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"Looking for feature for path '{context.Request.Path}'.");

            var feature = _featureProvider.GetFeature(context);
            if (feature == null)
            {
                _logger.LogWarning("No feature found.");
                await _next(context);
                return;
            }

            _logger.LogInformation($"Feature '{feature.Name}' was found.");

            if (!feature.HasChanged)
            {
                _logger.LogInformation("Feature has not changed.");
                SetEndpoint(context, feature);
                await _next(context);
                return;
            }

            context.Items["AAA"] = feature;

            var featureAssemblyRegex = new Regex(@$"{feature.Name}.\w+-\w+\-\w+\-\w+\-\w+");

            var parts = _appPartManager.ApplicationParts
                .OfType<AssemblyPart>()
                .Where(x => featureAssemblyRegex.IsMatch(x.Name))
                .ToArray();

            foreach (var part in parts)
            {
                _logger.LogInformation($"Removing assembly '{part.Assembly.FullName}' from ApplicationPartManager.");
                _appPartManager.ApplicationParts.Remove(part);
            }

            _logger.LogInformation($"Adding assembly '{feature.Assembly.FullName} to ApplicationPartManager.'");
            var assemblyPart = new AssemblyPart(feature.Assembly);
            _appPartManager.ApplicationParts.Add(assemblyPart);

            _logger.LogInformation("Triggering ActionDescriptonChangerProvider refresh.");
            _actionDescriptorChangeProvider.TokenSource.Cancel();

            SetEndpoint(context, feature);

            //context.Items[FeatureRuntimeCompilation.HttpContextItemKey] = feature.ControllerType;

            await _next(context);
        }

        private void SetEndpoint(HttpContext context, RuntimeFeatureProviderResult feature)
        {
            var eds = context.RequestServices.GetRequiredService<EndpointDataSource>();
            var endpoints = eds.Endpoints.Where(x => x.DisplayName.Contains(feature.Name, System.StringComparison.InvariantCultureIgnoreCase));
            context.SetEndpoint(endpoints.Last()); // TODO: match current assembly
        }
    }
}
