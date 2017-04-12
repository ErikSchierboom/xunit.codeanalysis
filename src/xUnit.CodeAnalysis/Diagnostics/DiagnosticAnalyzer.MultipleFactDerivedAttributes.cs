using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis.Diagnostics
{
    public partial class XUnitCodeAnalysisAnalyzer
    {
        public const string MultipleFactDerivedAttributesDiagnosticId = "MultipleFactDerivedAttributes";
        
        private static readonly DiagnosticDescriptor MultipleFactDerivedAttributesRule = new DiagnosticDescriptor(
            id: MultipleFactDerivedAttributesDiagnosticId,
            title: "Test method with multiple [Fact]-derived attributes",
            messageFormat: "Method '{0}' has multiple [Fact]-derived attributes",
            category: "xUnit.Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Test methods should not have multiple [Fact]-derived attributes."
        );

        private static bool MultipleFactDerivedAttributes(ImmutableArray<AttributeData> factDerivedAttributes) 
            => factDerivedAttributes.Length > 1;

        private static Diagnostic CreateMultipleFactDerivedAttributesDiagnostic(IMethodSymbol methodSymbol) 
            => Diagnostic.Create(MultipleFactDerivedAttributesRule, methodSymbol.Locations[0], methodSymbol.Name);
    }
}
