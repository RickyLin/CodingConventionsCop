using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.IO;

namespace CodingConventionsCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NameOfAwaitableMethodEndWithAsyncAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.NameOfAwaitableMethodEndWithAsync);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            foreach (SyntaxTree st in context.Compilation.SyntaxTrees.Where(t => !IsMvcController(t.FilePath)))
            {
                CompilationUnitSyntax rootSyntax = st.GetRoot() as CompilationUnitSyntax;
                if (rootSyntax == null)
                    continue;

                IEnumerable<MethodDeclarationSyntax> methodDeclarations = rootSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
                if (methodDeclarations.Count() == 0)
                    continue;
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(st);
                SymbolInfo symbolInfo;
                foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations)
                {
                    symbolInfo = semanticModel.GetSymbolInfo(methodDeclaration.ReturnType);
                    if (symbolInfo.Symbol.Name == "Task" && !methodDeclaration.Identifier.Text.EndsWith("Async"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NameOfAwaitableMethodEndWithAsync
                            , methodDeclaration.Identifier.GetLocation()));
                    }
                }
            }
        }

        private bool IsMvcController(string filePath)
        {
            /*
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            return fileName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                && !fileName.EndsWith("DataController", StringComparison.OrdinalIgnoreCase);
            */

            return filePath.EndsWith("Controller.cs", StringComparison.OrdinalIgnoreCase)
                && !filePath.EndsWith("DataController.cs", StringComparison.OrdinalIgnoreCase);
        }
    }
}
