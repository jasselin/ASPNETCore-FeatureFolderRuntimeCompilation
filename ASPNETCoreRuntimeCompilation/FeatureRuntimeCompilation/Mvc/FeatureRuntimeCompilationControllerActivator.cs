using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
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

            var feature = actionContext.HttpContext.Items["AAA"] as RuntimeFeatureProviderResult;

            //return actionContext.HttpContext.RequestServices.GetRequiredService(feature.ControllerType);
            return Activator.CreateInstance(feature.ControllerType);
        }

        public virtual void Release(ControllerContext context, object controller)
        {
        }
    }
}