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

        private static bool FactWithParameters(XUnitSymbolContext context)
            => context.HasParameters && context.HasFactAttribute;

        private static Diagnostic CreateFactWithParametersDiagnostic(XUnitSymbolContext context)
            => CreateDiagnostic(FactWithParametersRule, context);
    }
}
