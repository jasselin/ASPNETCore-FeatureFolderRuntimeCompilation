using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilationFailedException : Exception
    {
        public FeatureCompilationFailedException(string rootPath, FeatureCompilerResult result)
            : base(FormatMessage(rootPath, result.Failures))
        {
            CompilationFailures = result.Failures;
        }

        public IEnumerable<DiagnosticMessage> CompilationFailures { get; }

        private static string FormatMessage(string rootPath, IEnumerable<DiagnosticMessage> failures)
        {
            return "Compilation failed: " + Environment.NewLine +
                   string.Join(Environment.NewLine + Environment.NewLine, failures.Select(x => x.FormattedMessage.Replace(rootPath + @"\../", @"\")));
        }
    }
}
