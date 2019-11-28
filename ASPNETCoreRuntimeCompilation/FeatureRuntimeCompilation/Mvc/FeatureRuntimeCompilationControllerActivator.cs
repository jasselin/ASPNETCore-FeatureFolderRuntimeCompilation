using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    //TODO: Remove
    public class FeatureRuntimeCompilationControllerActivator : IControllerActivator
    {
        //private IRuntimeFeatureProvider _runtimeFeatureProvider;

        public FeatureRuntimeCompilationControllerActivator(/*IRuntimeFeatureProvider runtimeFeatureProvider*/)
        {
            //_runtimeFeatureProvider = runtimeFeatureProvider;
        }

        public object Create(ControllerContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            //var controllerType = _runtimeFeatureProvider.GetControllerType(actionContext);

            var controllerType = actionContext.HttpContext.Items[FeatureRuntimeCompilation.HttpContextItemKey] as Type;

            return actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
        }

        public virtual void Release(ControllerContext context, object controller)
        {
        }
    }
}
