using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Extensions
{
    public static class AppDomainExtensions
    {
        //TODO: Check if still useful
        public static IList<MetadataReference> GetReferences(this AppDomain appDomain)
        {
            var assemblies = appDomain.GetAssemblies().ToList();
            //assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);
            assemblies.Add(typeof(System.IO.Compression.ZipFile).Assembly);
            return assemblies
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .Cast<MetadataReference>()
                .ToList();
        }
    }
}
