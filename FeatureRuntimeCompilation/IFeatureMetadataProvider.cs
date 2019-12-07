using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FeatureRuntimeCompilation
{
    internal interface IFeatureMetadataProvider
    {
        FeatureMetadata GetMetadataFor(RouteValueDictionary routeValues);
        FeatureMetadata GetMetadataFor(string featurePath);
    }
}