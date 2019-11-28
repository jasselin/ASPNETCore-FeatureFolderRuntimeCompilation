using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Caching;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.DI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation
{
    public class RuntimeFeatureProvider : IRuntimeFeatureProvider
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFeatureCompilerCache _compilerCache;
        private readonly IFeatureRuntimeCompilationServiceProvider _serviceProvider;
        private readonly IFeatureMetadataProvider _featureMetadataProvider;

        public RuntimeFeatureProvider(IWebHostEnvironment env, IFeatureCompilerCache compilerCache, IFeatureRuntimeCompilationServiceProvider serviceProvider,
            IFeatureMetadataProvider featureMetadataProvider)
        {
            _env = env;
            _compilerCache = compilerCache;
            _serviceProvider = serviceProvider;
            _featureMetadataProvider = featureMetadataProvider;
        }

        //public Type GetControllerType(ControllerContext context)
        //{
        //    var featurePath = Path.Combine(_env.ContentRootPath, "Features", string.Join('/', context.ActionDescriptor.Properties.Values));
        //    if (!Directory.Exists(featurePath))
        //        return null;

        //    var controllerTypeName = context.ActionDescriptor.ControllerTypeInfo.FullName;

        //    var (cacheResult, newAssembly) = _compilerCache.GetOrAdd(controllerTypeName, featurePath);

        //    var compilerResult = cacheResult.Result;
        //    if (!compilerResult.Success)
        //        throw new FeatureCompilationFailedException(_env.ContentRootPath, compilerResult);

        //    var controllerType = compilerResult.Types.SingleOrDefault(x => x.FullName.Equals(controllerTypeName, StringComparison.InvariantCultureIgnoreCase));

        //    if (newAssembly)
        //    {
        //        //var services = compilerResult.Assembly.ExportedTypes.Select(x => (x, ServiceLifetime.Transient));
        //        //_serviceProvider.AddServices(services);
        //        var services = new List<ServiceDescriptor> { ServiceDescriptor.Transient(controllerType, controllerType) };
        //        _serviceProvider.AddServices(services);
        //    }

        //    return controllerType;
        //}

        //public RuntimeFeatureProviderResult GetFeature(RouteValueDictionary routeValues)
        //{
        //    var featurePathRouteValues = routeValues.Where(x => x.Key != "action" && x.Key != "controller").Select(x => x.Value); //TODO: refactor
        //    var featurePath = Path.Combine(_env.ContentRootPath, "Features", string.Join('\\', featurePathRouteValues));
        //    if (!Directory.Exists(featurePath))
        //        return null;

        //    var featureControllerRouteValues = routeValues.Where(x => x.Key != "action").Select(x => x.Value);
        //    var controllerTypeName = string.Concat(_env.ApplicationName, ".Features.", string.Join('.', featureControllerRouteValues), "Controller"); //TODO: refactor

        //    var (cacheResult, newAssembly) = _compilerCache.GetOrAdd(controllerTypeName, featurePath);

        //    var compilerResult = cacheResult.Result;
        //    if (!compilerResult.Success)
        //        throw new FeatureCompilationFailedException(_env.ContentRootPath, compilerResult);

        //    var controllerType = compilerResult.Types.SingleOrDefault(x => x.FullName.Equals(controllerTypeName, StringComparison.InvariantCultureIgnoreCase));

        //    return new RuntimeFeatureProviderResult(compilerResult.Assembly, newAssembly, controllerType);

        //    //if (newAssembly)
        //    //{
        //    //    //var services = compilerResult.Assembly.ExportedTypes.Select(x => (x, ServiceLifetime.Transient));
        //    //    //_serviceProvider.AddServices(services);
        //    //    //var services = new List<ServiceDescriptor> { ServiceDescriptor.Transient(controllerType, controllerType) };
        //    //    //_serviceProvider.AddServices(services);
        //    //}

        //    //return controllerType;
        //}

        public RuntimeFeatureProviderResult GetFeature(HttpContext context)
        {
            var metadata = _featureMetadataProvider.GetMetadataFor(context);
            if (metadata == null)
                return null;

            var (cacheResult, newAssembly) = _compilerCache.GetOrAdd(metadata.ControllerTypeName, metadata.FeaturePath);

            var compilerResult = cacheResult.Result;
            if (!compilerResult.Success)
                throw new FeatureCompilationFailedException(_env.ContentRootPath, compilerResult);

            var controllerType = compilerResult.Types.SingleOrDefault(x => x.FullName.Equals(metadata.ControllerTypeName, StringComparison.InvariantCultureIgnoreCase));

            return new RuntimeFeatureProviderResult(compilerResult.Assembly, newAssembly, controllerType);
        }
    }
}