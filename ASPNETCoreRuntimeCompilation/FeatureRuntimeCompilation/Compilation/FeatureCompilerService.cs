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
using System.Reflection;
using System.Threading;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Compilation
{
    public class FeatureCompilerService : IFeatureCompilerService
    {
        private IList<MetadataReference> _compilationReferences;
        private bool _compilationReferencesInitialized;
        private object _compilationReferencesLock = new object();
        private readonly FeatureRuntimeCompilationOptions _options;
        private readonly RazorProjectEngine _razorProjectEngine;
        private IList<MetadataReference> _refs;

        public FeatureCompilerService(FeatureRuntimeCompilationOptions options, RazorProjectEngine razorProjectEngine)
        {
            _options = options;
            _razorProjectEngine = razorProjectEngine;
        }

        private CSharpCompilation GetCompilation(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);

            var references = new List<MetadataReference>(CompilationReferences.Where(x => !(x.Display.Contains("Prextra.") && x.Display.Contains(".Web.dll"))));

            var compilation = CSharpCompilation.Create(assemblyName, options: compileOptions, syntaxTrees: syntaxTrees, references: references);
            return Rewrite(compilation);
        }

        public FeatureCompilerResult Compile(string controllerDir)
        {
            var csFiles = Directory.GetFiles(controllerDir, "*.cs", SearchOption.AllDirectories);
            var assemblyName = Guid.NewGuid().ToString();

            var syntaxTrees = csFiles.Select(file =>
            {
                using (var stream = File.OpenRead(file))
                {
                    var text = SourceText.From(stream);
                    return CSharpSyntaxTree.ParseText(text, null, file);
                }
            });

            var compilation = GetCompilation(assemblyName, syntaxTrees);

            var outputPath = Path.Combine(_options.ProjectPath, "Temp", "dynamic_assemblies");
            Directory.CreateDirectory(outputPath);

            //TODO: In memory only
            var assemblyPath = Path.Combine(outputPath, assemblyName + ".dll");
            var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");

            using (var assemblyStream = new FileStream(assemblyPath, FileMode.Create))
            {
                using (var pdbStream = new FileStream(pdbPath, FileMode.Create))
                {
                    var result = compilation.Emit(assemblyStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.Pdb));
                    if (!result.Success)
                        return GetCompilationFailedResult(result.Diagnostics);
                }
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            return new FeatureCompilerResult(assembly);
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
