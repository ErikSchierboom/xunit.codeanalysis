using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis.Diagnostics
{
    public partial class XUnitCodeAnalysisAnalyzer
    {
        public const string TheoryWithoutParametersDiagnosticId = "TheoryWithoutParameters";
        
        private static readonly DiagnosticDescriptor TheoryWithoutParametersRule = new DiagnosticDescriptor(
            id: TheoryWithoutParametersDiagnosticId, 
            title: "[Theory] method without parameters", 
            messageFormat: "[Theory] methods must have one or more parameters", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true, 
            description: "[Theory] methods should have one or more parameters."
        );

        private static bool TheoryWithoutParameters(ImmutableArray<AttributeData> factDerivedAttributes, INamedTypeSymbol theoryAttribute, IMethodSymbol methodSymbol) 
            => !methodSymbol.Parameters.Any() && factDerivedAttributes.Any(f => f.AttributeClass.Equals(theoryAttribute));

        private static Diagnostic CreateTheoryWithoutParametersDiagnostic(IMethodSymbol methodSymbol)
            => Diagnostic.Create(TheoryWithoutParametersRule, methodSymbol.Locations[0], methodSymbol.Name);
    }
}
