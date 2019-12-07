using System;
using System.Collections.Generic;
using System.Reflection;

namespace FeatureRuntimeCompilation
{
    // Help resolve dynamically created assemblies
    internal static class FeatureAssemblyLocator
    {
        private static Dictionary<string, Assembly> _assemblies;

        public static void Init()
        {
            _assemblies = new Dictionary<string, Assembly>();
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            _assemblies.TryGetValue(args.Name, out var assembly);
            return assembly;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            _assemblies[assembly.FullName] = assembly;
        }
    }
}