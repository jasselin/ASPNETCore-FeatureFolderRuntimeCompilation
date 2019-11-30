using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration;
using ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerService : IFeatureCompilerService
    {
        private IList<MetadataReference> _compilationReferences;
        private bool _compilationReferencesInitialized;
        private object _compilationReferencesLock = new object();
        private IList<MetadataReference> _refs;

        private readonly RazorProjectEngine _razorProjectEngine; //TODO: get references elsewhere?
        private readonly FeatureRuntimeCompilationOptions _options;

        public FeatureCompilerService(RazorProjectEngine razorProjectEngine, FeatureRuntimeCompilationOptions options)
        {
            _razorProjectEngine = razorProjectEngine;
            _options = options;
        }

        private CSharpCompilation GetCompilation(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default); //TODO: still useful?

            //TODO: Remove Prextra hack
            var references = new List<MetadataReference>(CompilationReferences.Where(x => !(x.Display.Contains("Prextra.") && x.Display.Contains(".Web.dll"))));

            var compilation = CSharpCompilation.Create(assemblyName, options: compileOptions, syntaxTrees: syntaxTrees, references: references);
            return Rewrite(compilation);
        }

        public FeatureCompilerResult Compile(string assemblyName, string controllerDir)
        {
            Stream CreateStream(string filePath)
            {
                if (_options.UseInMemoryAssemblies)
                    return new MemoryStream();

                return new FileStream(filePath, FileMode.Create);
            }

            var tempAssemblyName = string.Concat(assemblyName, ".", Guid.NewGuid().ToString());

            var assemblyFilePath = Path.Combine(_options.AssembliesOutputPath, string.Concat(tempAssemblyName, ".dll"));
            var pdbFilePath = Path.ChangeExtension(assemblyFilePath, "pdb");

            using var assemblyStream = CreateStream(assemblyFilePath);
            using var pdbStream = CreateStream(pdbFilePath);

            var syntaxTrees = GetSyntaxTrees(controllerDir);
            var result = GetCompilation(tempAssemblyName, syntaxTrees)
                .Emit(assemblyStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.Pdb));

            if (!result.Success)
                return GetCompilationFailedResult(result.Diagnostics);

            var assemblyLoadContext = CreateAssemblyLoadContext(tempAssemblyName, assemblyFilePath, assemblyStream, pdbStream);

            return new FeatureCompilerResult(assemblyLoadContext);
        }

        private AssemblyLoadContext CreateAssemblyLoadContext(string contextName, string assemblyFilePath, Stream assemblyStream, Stream pdbStream)
        {
            var assemblyLoadContext = new AssemblyLoadContext(contextName, true);

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
                using (var stream = File.OpenRead(file))
                {
                    var text = SourceText.From(stream);
                    return CSharpSyntaxTree.ParseText(text, null, file);
                }
            });
            return syntaxTrees;
        }

        private IEnumerable<MetadataReference> CompilationReferences => LazyInitializer.EnsureInitialized(ref _compilationReferences,
                                                                                                          ref _compilationReferencesInitialized,
                                                                                                          ref _compilationReferencesLock,
                                                                                                          GetCompilationReferences);

        private IList<MetadataReference> GetCompilationReferences()
        {
            if (_refs == null)
                _refs = AppDomain.CurrentDomain.GetReferences();

            var metadataReferenceFeature = _razorProjectEngine.EngineFeatures.SingleOrDefault(x => x is IMetadataReferenceFeature) as IMetadataReferenceFeature;

            return metadataReferenceFeature.References.Union(_refs).Distinct().ToList();
        }

        //TODO: Remove?
        private static CSharpCompilation Rewrite(CSharpCompilation compilation)
        {
            //var rewrittenTrees = new List<SyntaxTree>();
            //foreach (var tree in compilation.SyntaxTrees)
            //{
            //    var semanticModel = compilation.GetSemanticModel(tree, true);
            //    var rewriter = new ExpressionRewriter(semanticModel);

            //    var rewrittenTree = tree.WithRootAndOptions(rewriter.Visit(tree.GetRoot()), tree.Options);
            //    rewrittenTrees.Add(rewrittenTree);
            //}

            //return compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(rewrittenTrees);
            return compilation;
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
