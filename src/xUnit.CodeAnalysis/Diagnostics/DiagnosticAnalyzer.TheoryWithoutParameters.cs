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

        private static bool TheoryWithoutParameters(XUnitSymbolContext context) 
            => !context.HasParameters && context.HasTheoryAttribute;

        private static Diagnostic CreateTheoryWithoutParametersDiagnostic(XUnitSymbolContext context)
            => CreateDiagnostic(TheoryWithoutParametersRule, context);
    }
}
