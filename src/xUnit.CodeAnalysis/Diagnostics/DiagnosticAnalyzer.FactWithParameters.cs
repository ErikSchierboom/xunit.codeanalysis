using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis.Diagnostics
{
    public partial class XUnitCodeAnalysisAnalyzer
    {
        public const string FactWithParametersDiagnosticId = "FactWithParameters";
        
        private static readonly DiagnosticDescriptor FactWithParametersRule = new DiagnosticDescriptor(
            id: FactWithParametersDiagnosticId, 
            title: "[Fact] method with parameters", 
            messageFormat: "[Fact] methods are not allowed to have parameters", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true, 
            description: "[Fact] methods should not have parameters."
        );

        private static bool FactWithParameters(ImmutableArray<AttributeData> factDerivedAttributes, INamedTypeSymbol factAttribute, IMethodSymbol methodSymbol) 
            => methodSymbol.Parameters.Any() && factDerivedAttributes.Any(f => f.AttributeClass.Equals(factAttribute));

        private static Diagnostic CreateFactWithParametersDiagnostic(IMethodSymbol methodSymbol)
            => Diagnostic.Create(FactWithParametersRule, methodSymbol.Locations[0], methodSymbol.Name);
    }
}
