using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Immutable;

namespace CodingConventionsCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OneFileOneTypeDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.OneFileOneTypeDeclaration);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeCompilation);
        }

        private void AnalyzeCompilation(SyntaxTreeAnalysisContext context)
        {
            CompilationUnitSyntax rootSyntax = context.Tree.GetRoot() as CompilationUnitSyntax;
            if (rootSyntax == null)
                return;

            IEnumerable<BaseTypeDeclarationSyntax> typeDeclarations = rootSyntax.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
            int typeDeclarationsCount = typeDeclarations.Count();
            if (typeDeclarationsCount > 1)
            {
                int index = 0;
                foreach (BaseTypeDeclarationSyntax td in typeDeclarations)
                {
                    if (index++ == 0)
                        continue;
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.OneFileOneTypeDeclaration, td.GetLocation()));
                }
            }

        }
    }
}
