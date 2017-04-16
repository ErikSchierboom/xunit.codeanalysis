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

        private static bool InlineDataWithoutTheory(XUnitSymbolContext context)
            => context.HasInlineDataAttribute && !context.HasTheoryAttribute;

        private static Diagnostic CreateInlineDataWithoutTheoryDiagnostic(XUnitSymbolContext context)
            => CreateDiagnostic(InlineDataWithoutTheoryRule, context);
    }
}
