using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureFolders
{
    public class FeatureControllerModelConvention : IControllerModelConvention
    {
        private readonly FeatureRuntimeCompilationOptions _options;
        private readonly ILogger<FeatureControllerModelConvention> _logger;

        public FeatureControllerModelConvention(FeatureRuntimeCompilationOptions options, ILogger<FeatureControllerModelConvention> logger)
        {
            _options = options;
            _logger = logger;
        }

        public void Apply(ControllerModel controller)
        {
            _logger.LogDebug($"FeatureControllerModelConvention.Apply ('{controller.ControllerType.FullName}')");

            SetControllerProperties(controller);
            SetControllerRoute(controller);
        }
        
        private void SetControllerProperties(ControllerModel controller)
        {
            var controllerType = controller.ControllerType;

            // Not a feature controller
            if (!controllerType.FullName.StartsWith(_options.FeatureNamespace))
                return;

            // Gets the controller properies from the feature class name
            // ASPNETCoreRuntimeCompilation.Features.Level1.Level2.FeatureA => { level1 = "Level1", level2 = "Level2" }
            var tokens = controllerType.FullName.Substring(_options.FeatureNamespace.Length + 1).Split('.');
            for (var i = 0; i < tokens.Length - 1; i++)
            {
                var key = $"level{tokens.Length - i - 1}";
                controller.RouteValues.Add(key, tokens[i]); // needed by FeatureMetadataProvider
                controller.Properties.Add(key, tokens[i]);
            }
        }

        private void SetControllerRoute(ControllerModel controller)
        {
            // Sets the controller route using a convention via RouteAttribute
            // ASPNETCoreRuntimeCompilation.Features.Level2.Level1.FeatureA => Route("Level2/Level1/FeatureA/{action=Index}")
            foreach (var selector in controller.Selectors.Where(x => x.AttributeRouteModel == null))
            {
                var controllerPath = string.Join('/', controller.Properties.Values);
                var attrRouteModel = new AttributeRouteModel
                {
                    Template = string.Concat(controllerPath, "/{action=Index}")
                };
                selector.AttributeRouteModel = attrRouteModel;
            }
        }
    }
}
