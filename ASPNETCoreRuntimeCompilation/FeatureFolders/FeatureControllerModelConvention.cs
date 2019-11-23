using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureFolders
{
    public class FeatureControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            SetControllerProperties(controller);
            SetControllerRoute(controller);
        }
        
        private void SetControllerProperties(ControllerModel controller)
        {
            var controllerType = controller.ControllerType;
            var featureNamespace = string.Concat(controllerType.Assembly.GetName().Name, ".Features.");
            if (!controllerType.FullName.StartsWith(featureNamespace))
                return;

            var tokens = controllerType.FullName.Substring(featureNamespace.Length).Split('.');
            for (var i = 0; i < tokens.Length - 1; i++)
                controller.Properties.Add($"level{tokens.Length - i - 1}", tokens[i]);
        }

        private void SetControllerRoute(ControllerModel controller)
        {
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
