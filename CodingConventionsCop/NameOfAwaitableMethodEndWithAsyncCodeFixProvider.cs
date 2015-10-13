using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Composition;

namespace CodingConventionsCop
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndentWithFourSpacesCodeFixProvider))]
    [Shared]
    public class NameOfAwaitableMethodEndWithAsyncCodeFixProvider : CodeFixProvider
    {
        private const string CODE_FIX_TITLE = "Append \"Async\" suffix to method name";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIds.NameOfAwaitableMethodEndWithAsync);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            Diagnostic diagnostic = context.Diagnostics.First();
            SyntaxToken methodNameToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix
                (
                    CodeAction.Create
                    (
                        CODE_FIX_TITLE,
                        c => AppendAsyncSuffixToMethodNameAsync(context.Document, root, methodNameToken, c),
                        CODE_FIX_TITLE
                    ),
                    diagnostic
                );
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private Task<Document> AppendAsyncSuffixToMethodNameAsync(Document document, SyntaxNode root, SyntaxToken methodNameToken
            , CancellationToken cancellationToken)
        {
            string methodName = methodNameToken.Text;
            string newMethodName = (methodName.EndsWith("async") ? methodName.Substring(0, methodName.Length - 5) : methodName)
                + "Async";
            SyntaxNode newRoot = root.ReplaceToken(methodNameToken, SyntaxFactory.Identifier(newMethodName));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}