using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodingConventionsCop.Tests
{
    public abstract class DiagnosticVerifier
    {
        private const string ADHOC_PROJECT_NAME = "AdHocProjectGeneratedForTesting";
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        protected abstract DiagnosticAnalyzer GetDiagnosticAnalyzer();

        protected void VerifyNoDiagnostic(string sourceCode)
        {
            VerifyDiagnostic(new[] { sourceCode }, new string[0]);
        }

        protected void VerifyDiagnostic(string sourceCode, string expectedDiagnosticId)
        {
            VerifyDiagnostic(new[] { sourceCode }, new[] { expectedDiagnosticId });
        }

        protected void VerifyDiagnostic(string sourceCode, IEnumerable<string> expectedDiagnosticIds)
        {
            VerifyDiagnostic(new[] { sourceCode }, expectedDiagnosticIds);
        }

        protected void VerifyDiagnostic(IEnumerable<string> sourceCodes, IEnumerable<string> expectedDiagnosticIds)
        {
            Diagnostic[] diagnostics = GetDiagnostics(sourceCodes);
            if (expectedDiagnosticIds.Count() == 0)
                Assert.AreEqual(0, diagnostics.Length);

            string actual = string.Join(",", diagnostics.Select(d => d.Id));
            string expected = string.Join(",", expectedDiagnosticIds.OrderBy(id => id));
            Assert.AreEqual(expected, actual);
        }

        protected Diagnostic[] GetDiagnostics(IEnumerable<string> sourceCodes)
        {
            DiagnosticAnalyzer analyzer = GetDiagnosticAnalyzer();
            Project project = CreateProject(sourceCodes);
            CompilationWithAnalyzers compilationWithAnalyzers = project.GetCompilationAsync().Result
                .WithAnalyzers(ImmutableArray.Create(analyzer));
            ImmutableArray<Diagnostic> analyzerDiagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
            List<Diagnostic> diagnostics = new List<Diagnostic>(analyzerDiagnostics.Length);
            foreach (Diagnostic diagnostic in analyzerDiagnostics)
            {
                if (diagnostic.Location == Location.None || diagnostic.Location.IsInMetadata)
                    diagnostics.Add(diagnostic);
                else
                {
                    SyntaxTree tree;
                    foreach (Document doc in project.Documents)
                    {
                        tree = doc.GetSyntaxTreeAsync().Result;
                        if (tree == diagnostic.Location.SourceTree)
                            diagnostics.Add(diagnostic);
                    }
                }
            }

            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        private static Project CreateProject(IEnumerable<string> sourceCodes)
        {
            ProjectId projectId = ProjectId.CreateNewId(debugName: ADHOC_PROJECT_NAME);
            Solution solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, ADHOC_PROJECT_NAME, ADHOC_PROJECT_NAME, "C#")
                .AddMetadataReferences(projectId, new[]
                {
                    CorlibReference,
                    SystemCoreReference,
                    CSharpSymbolsReference,
                    CodeAnalysisReference
                });

            string sourceFileName;
            DocumentId documentId;
            int index = 0;
            foreach (string sourceCode in sourceCodes)
            {
                sourceFileName = $"Test{index++}.cs";
                documentId = DocumentId.CreateNewId(projectId, debugName: sourceFileName);
                solution = solution.AddDocument(documentId, sourceFileName, SourceText.From(sourceCode));
            }

            return solution.GetProject(projectId);
        }
    }
}
