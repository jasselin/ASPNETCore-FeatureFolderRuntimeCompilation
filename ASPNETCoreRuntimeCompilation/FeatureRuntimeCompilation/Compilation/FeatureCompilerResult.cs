using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerResult
    {
        public FeatureCompilerResult(IEnumerable<DiagnosticMessage> failures)
        {
            Failures = failures;
        }

        public FeatureCompilerResult(WeakReference assemblyLoadContextRef)
        {
            AssemblyLoadContextRef = assemblyLoadContextRef;
            Failures = new List<DiagnosticMessage>();
        }

        public WeakReference AssemblyLoadContextRef { get; }
        public Assembly Assembly
        {
            get
            {
                var assemblyLoadContext = AssemblyLoadContextRef.Target as FeatureAssemblyLoadContext;
                return assemblyLoadContext.Assemblies.First();
            }
        }
        //public Assembly Assembly => AssemblyLoadContext.Assemblies.First();
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes.Select(x => x.GetTypeInfo());
        public IEnumerable<DiagnosticMessage> Failures { get; }
        public bool Success => !Failures.Any();
    }
}
