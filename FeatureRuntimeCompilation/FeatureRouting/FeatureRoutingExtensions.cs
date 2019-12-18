using FeatureRuntimeCompilation.Caching;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace FeatureRuntimeCompilation.FeatureRouting
{
    public static class FeatureRoutingExtensions
    {
        public static ControllerActionEndpointConventionBuilder MapFeatureControllerRoute(
            this IEndpointRouteBuilder endpoints, object values)
        {
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            //EnsureControllerServices(endpoints);

            var routeValues = new RouteValueDictionary(values);

            var url = string.Join('/', routeValues.Values);
            var pattern = string.Concat(url, "/{action=Index}");

            var metadataProvider = endpoints.ServiceProvider.GetRequiredService<IFeatureMetadataProvider>();
            var feature = metadataProvider.GetMetadataFor(routeValues);

            if (feature == null)
                throw new Exception($"No feature found for path '{url}'.");

            var dataSource = GetOrCreateDataSource(endpoints, feature);
            return dataSource.AddRoute(
                url,
                pattern,
                routeValues,
                new RouteValueDictionary(null /*constraints*/),
                new RouteValueDictionary(null /*dataTokens*/));
        }

        private static FeatureControllerActionEndpointDataSource GetOrCreateDataSource(IEndpointRouteBuilder endpoints, FeatureMetadata feature)
        {
            var actions = endpoints.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
            var endpointFactory = endpoints.ServiceProvider.GetRequiredService<ActionEndpointFactory>();
            var tokenProvider = endpoints.ServiceProvider.GetRequiredService<IFeatureChangeTokenProvider>();

            Func<(CancellationTokenSource, IChangeToken)> getToken = () => tokenProvider.GetToken(feature);

            var dataSource = new FeatureControllerActionEndpointDataSource(actions, endpointFactory, getToken);
            endpoints.DataSources.Add(dataSource);

            return dataSource;
        }
    }
}
