using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis.Diagnostics
{
    public partial class XUnitCodeAnalysisAnalyzer
    {
        public const string InlineDataWithoutTheoryDiagnosticId = "InlineDataWithoutTheory";
        
        private static readonly DiagnosticDescriptor InlineDataWithoutTheoryRule = new DiagnosticDescriptor(
            id: InlineDataWithoutTheoryDiagnosticId, 
            title: "[InlineData] specified without [Theory]", 
            messageFormat: "[InlineData] should be accompanied by [Theory]", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "[InlineData] should have [Theory]."
        );

        private static bool InlineDataWithoutTheory(ImmutableArray<AttributeData> factDerivedAttributes, INamedTypeSymbol theoryAttribute, INamedTypeSymbol inlineDataAttribute, IMethodSymbol methodSymbol) 
            => methodSymbol.GetAttributes().Any(f => f.AttributeClass.Equals(inlineDataAttribute)) && !factDerivedAttributes.Any(f => f.AttributeClass.Equals(theoryAttribute));

        private static Diagnostic CreateInlineDataWithoutTheoryDiagnostic(IMethodSymbol methodSymbol)
            => Diagnostic.Create(InlineDataWithoutTheoryRule, methodSymbol.Locations[0], methodSymbol.Name);
    }
}
