using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureProvider : IRuntimeFeatureProvider
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFeatureCompilerCache _compilerCache;

        public RuntimeFeatureProvider(IWebHostEnvironment env, IFeatureCompilerCache compilerCache)
        {
            _env = env;
            _compilerCache = compilerCache;
        }

        public Type GetControllerType(ControllerContext context)
        {
            var featurePath = Path.Combine(_env.ContentRootPath, "Features", string.Join('/', context.ActionDescriptor.Properties.Values));
            if (!Directory.Exists(featurePath))
                return null;

            var controllerTypeName = context.ActionDescriptor.ControllerTypeInfo.FullName;

            var result = _compilerCache.GetOrAdd(controllerTypeName, featurePath).Result;
            if (!result.Success)
                throw new FeatureCompilationFailedException(_env.ContentRootPath, result);

            return result.Types.SingleOrDefault(x => x.FullName.Equals(controllerTypeName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}