using Microsoft.AspNetCore.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerResult
    {
        public FeatureCompilerResult(IEnumerable<DiagnosticMessage> failures)
        {
            Failures = failures;
        }

        public FeatureCompilerResult(Assembly assembly)
        {
            Assembly = assembly;
            Failures = new List<DiagnosticMessage>();
        }

        public Assembly Assembly { get; }
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes.Select(x => x.GetTypeInfo());
        public IEnumerable<DiagnosticMessage> Failures { get; }
        public bool Success => !Failures.Any();
    }
}
