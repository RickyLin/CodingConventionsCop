using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingConventionsCop
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndentWithFourSpacesCodeFixProvider))]
    [Shared]
    public class IndentWithFourSpacesCodeFixProvider : CodeFixProvider
    {
        private const string CODE_FIX_TITLE = "Replace the tab with 4 spaces";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIds.IndentWithFourSpaces);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            Diagnostic diagnostic = context.Diagnostics.First();
            SyntaxTrivia tabTrivia = root.FindTrivia(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix
                (
                    CodeAction.Create(
                        CODE_FIX_TITLE,
                        c => ReplaceTabWithFourSpaces(context.Document, root, tabTrivia, c),
                        CODE_FIX_TITLE),
                    diagnostic
                );
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private Task<Document> ReplaceTabWithFourSpaces(Document document, SyntaxNode root, SyntaxTrivia tabTrivia
            , CancellationToken cancellationToken)
        {
            SyntaxTrivia fourSpacesTrivia = SyntaxFactory.Whitespace("    ");
            SyntaxNode newRoot = root.ReplaceTrivia(tabTrivia, fourSpacesTrivia);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
