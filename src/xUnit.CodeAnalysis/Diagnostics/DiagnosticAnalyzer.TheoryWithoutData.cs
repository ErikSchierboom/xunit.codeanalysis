using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis.Diagnostics
{
    public partial class XUnitCodeAnalysisAnalyzer
    {
        public const string TheoryWithoutDataDiagnosticId = "TheoryWithoutData";
        
        private static readonly DiagnosticDescriptor TheoryWithoutDataRule = new DiagnosticDescriptor(
            id: TheoryWithoutDataDiagnosticId, 
            title: "[Theory] method without data", 
            messageFormat: "[Theory] methods must have one or more [Data]-derived attributes", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true, 
            description: "[Theory] methods must have one or more [Data]-derived attributes."
        );

        private static bool TheoryWithoutData(XUnitSymbolContext context) 
            => context.HasParameters && context.HasTheoryAttribute && !context.HasDataAttribute;

        private static Diagnostic CreateTheoryWithoutDataDiagnostic(XUnitSymbolContext context)
            => CreateDiagnostic(TheoryWithoutDataRule, context);
    }
}
