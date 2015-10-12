using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConventionsCop
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor OneFileOneTypeDeclaration = new DiagnosticDescriptor
        (
            id: DiagnosticIds.OneFileOneTypeDeclaration,
            title: "Over one type declared in the source code file",
            messageFormat: "Declare only one type in a source code file",
            category: DiagnosticCategories.CodingConvention,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NameOfAwaitableMethodEndWithAsync = new DiagnosticDescriptor
        (
            id: DiagnosticIds.NameOfAwaitableMethodEndWithAsync,
            title: "The method name should end with \"Async\" suffix",
            messageFormat: "The name of an awaitable method should end with \"Async\" suffix",
            category: DiagnosticCategories.CodingConvention,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor IndentWithFourSpaces = new DiagnosticDescriptor
        (
            id: DiagnosticIds.IndentWithFourSpaces,
            title: "Indent with 4 spaces rather than 1 tab",
            messageFormat: "Indent with 4 spaces instead of 1 tab",
            category: DiagnosticCategories.CodingConvention,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );
    }
}
