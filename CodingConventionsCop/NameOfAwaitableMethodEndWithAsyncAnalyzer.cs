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
            context.RegisterSemanticModelAction(AnalyzeSemanticModel);
        }

        private void AnalyzeSemanticModel(SemanticModelAnalysisContext context)
        {
            SyntaxTree st = context.SemanticModel.SyntaxTree;
            if (IgnoreSourceDocumentFile(st.FilePath))
                return;

            CompilationUnitSyntax rootSyntax = st.GetRoot() as CompilationUnitSyntax;
            if (rootSyntax == null)
                return;

            IEnumerable<MethodDeclarationSyntax> methodDeclarations = rootSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
            if (methodDeclarations.Count() == 0)
                return;

            SymbolInfo symbolInfo;
            foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations)
            {
                symbolInfo = context.SemanticModel.GetSymbolInfo(methodDeclaration.ReturnType);
                if (symbolInfo.Symbol?.Name == "Task" && !methodDeclaration.Identifier.Text.EndsWith("Async"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NameOfAwaitableMethodEndWithAsync
                        , methodDeclaration.Identifier.GetLocation()));
                }
            }
        }

        private bool IgnoreSourceDocumentFile(string filePath)
        {
            bool isController = filePath.EndsWith("Controller.cs", StringComparison.OrdinalIgnoreCase)
                && !filePath.EndsWith("DataController.cs", StringComparison.OrdinalIgnoreCase);

            bool isMiddleware = filePath.EndsWith("Middleware.cs", StringComparison.OrdinalIgnoreCase);

            return isController || isMiddleware;
        }
    }
}
