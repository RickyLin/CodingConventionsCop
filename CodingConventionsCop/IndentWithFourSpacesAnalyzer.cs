using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CodingConventionsCop
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IndentWithFourSpacesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.IndentWithFourSpaces);

        public override void Initialize(AnalysisContext context)
        {
            /*
            // this doesn't work, read https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/285
            // SyntaxKind.WhitespaceTrivia is not SyntaxNode, it's SyntaxTrivia.
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.WhitespaceTrivia);
            */
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root;
            if (!context.Tree.TryGetRoot(out root))
                return;
            IEnumerable<SyntaxTrivia> tabTrivias = root.DescendantTrivia(descendIntoTrivia: true)
                .Where(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia) && trivia.ToString().IndexOf('\t') >= 0);
            foreach (SyntaxTrivia st in tabTrivias)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.IndentWithFourSpaces, st.GetLocation()));
            }
        }

        /*
        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.GetText().ToString().IndexOf('\t') > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.IndentWithFourSpaces, context.Node.GetLocation()));
            }
        }
        */
    }
}
