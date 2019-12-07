using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;

namespace FeatureRuntimeCompilation.FeatureFolders
{
    internal class FeatureViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (viewLocations == null)
                throw new ArgumentNullException(nameof(viewLocations));

            if (!(context.ActionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                throw new NullReferenceException("ControllerActionDescriptor cannot be null.");

            // Default view locations ("~/Views", "~/Views/Shared")
            foreach (var location in viewLocations)
                yield return location;

            // Feature view ("/Features/levelN+1/levelN/FeatureName")
            var defaultViewLocation = string.Concat("/Features/", string.Join('/', controllerActionDescriptor.Properties.Values), "/{0}.cshtml");
            yield return defaultViewLocation;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // Different views with same name on different paths gets resolved by the cache at the first accessed location.
            // see: https://stackoverflow.com/questions/36802661/what-is-iviewlocationexpander-populatevalues-for-in-asp-net-core-mvc
            context.Values["action_displayname"] = context.ActionContext.ActionDescriptor.DisplayName;
        }
    }
}
