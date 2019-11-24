using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public interface IRuntimeFeatureProvider
    {
        Type GetControllerType(ControllerContext context);
    }
}