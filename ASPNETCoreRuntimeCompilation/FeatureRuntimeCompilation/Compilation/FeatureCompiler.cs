using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    internal class FeatureCompiler : IFeatureCompiler
    {
        private readonly RazorReferenceManager _referenceManager;
        private readonly FeatureRuntimeCompilationOptions _options;

        public FeatureCompiler(RazorReferenceManager referenceManager, FeatureRuntimeCompilationOptions options)
        {
            _referenceManager = referenceManager;
            _options = options;
        }

        private CSharpCompilation GetCompilation(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            return CSharpCompilation.Create(assemblyName, options: compileOptions, syntaxTrees: syntaxTrees, references: _referenceManager.CompilationReferences);
        }

        public FeatureCompilerResult Compile(string assemblyName, string featurePath, string checksum)
        {
            Stream CreateStream(string filePath)
            {
                if (_options.UseInMemoryAssemblies)
                    return new MemoryStream();

                // Sometimes, directory not created before first request triggers compilation
                if (!Directory.Exists(_options.AssembliesOutputPath))
                    Directory.CreateDirectory(_options.AssembliesOutputPath);

                return new FileStream(filePath, FileMode.Create);
            }

            var tempAssemblyName = string.Concat(assemblyName, ".", Guid.NewGuid().ToString());

            var assemblyFilePath = Path.Combine(_options.AssembliesOutputPath, string.Concat(tempAssemblyName, ".dll"));
            var pdbFilePath = Path.ChangeExtension(assemblyFilePath, "pdb");

            using var assemblyStream = CreateStream(assemblyFilePath);
            using var pdbStream = CreateStream(pdbFilePath);

            var syntaxTrees = GetSyntaxTrees(featurePath);
            var result = GetCompilation(tempAssemblyName, syntaxTrees)
                .Emit(assemblyStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.Pdb));

            if (!result.Success)
                return GetCompilationFailedResult(result.Diagnostics);

            var assemblyLoadContext = CreateAssemblyLoadContext(tempAssemblyName, assemblyFilePath, assemblyStream, pdbStream);

            return new FeatureCompilerResult(assemblyLoadContext, checksum);
        }

        private AssemblyLoadContext CreateAssemblyLoadContext(string contextName, string assemblyFilePath, Stream assemblyStream, Stream pdbStream)
        {
            var assemblyLoadContext = new AssemblyLoadContext(contextName, false); // resolving collectible assembly not supported

            if (_options.UseInMemoryAssemblies)
            {
                // Streams must be set to position 0 before reading (Bad IL Format exception)
                assemblyStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);

                assemblyLoadContext.LoadFromStream(assemblyStream, pdbStream);
            }
            else
            {
                // Close the file streams before loading
                assemblyStream.Close();
                pdbStream.Close();

                assemblyLoadContext.LoadFromAssemblyPath(assemblyFilePath);
            }

            return assemblyLoadContext;
        }

        private static IEnumerable<SyntaxTree> GetSyntaxTrees(string controllerDir)
        {
            var csFiles = Directory.GetFiles(controllerDir, "*.cs", SearchOption.AllDirectories);

            var syntaxTrees = csFiles.Select(file =>
            {
                // FileShare.ReadWrite to prevent IOException (cannot access file...)
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var text = SourceText.From(stream);
                    return CSharpSyntaxTree.ParseText(text, null, file);
                }
            });
            return syntaxTrees;
        }

        private FeatureCompilerResult GetCompilationFailedResult(IEnumerable<Diagnostic> diagnostics)
        {
            var failures = diagnostics
                .Where(x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error)
                .Select(GetDiagnosticMessage);

            return new FeatureCompilerResult(failures);
        }

        private static DiagnosticMessage GetDiagnosticMessage(Diagnostic diagnostic)
        {
            var mappedLineSpan = diagnostic.Location.GetMappedLineSpan();
            return new DiagnosticMessage(
                diagnostic.GetMessage(),
                CSharpDiagnosticFormatter.Instance.Format(diagnostic),
                mappedLineSpan.Path,
                mappedLineSpan.StartLinePosition.Line + 1,
                mappedLineSpan.StartLinePosition.Character + 1,
                mappedLineSpan.EndLinePosition.Line + 1,
                mappedLineSpan.EndLinePosition.Character + 1);
        }
    }
}
