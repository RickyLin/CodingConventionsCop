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
        public static readonly DiagnosticDescriptor OneFileOneTypeDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIds.OneFileOneTypeDeclaration,
            title: "Over one type in source code file",
            messageFormat: "Declare only one type in the source code file",
            category: DiagnosticCategories.CodingConvention,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
            );
    }
}
