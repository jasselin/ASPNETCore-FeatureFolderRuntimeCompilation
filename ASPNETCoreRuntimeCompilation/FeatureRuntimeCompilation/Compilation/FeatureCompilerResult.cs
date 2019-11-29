using Microsoft.AspNetCore.Diagnostics;
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

        public FeatureCompilerResult(AssemblyLoadContext assemblyLoadContext)
        {
            AssemblyLoadContext = assemblyLoadContext;
            Failures = new List<DiagnosticMessage>();
        }

        public AssemblyLoadContext AssemblyLoadContext { get; }
        public Assembly Assembly => AssemblyLoadContext.Assemblies.First();
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes.Select(x => x.GetTypeInfo());
        public IEnumerable<DiagnosticMessage> Failures { get; }
        public bool Success => !Failures.Any();
    }
}
