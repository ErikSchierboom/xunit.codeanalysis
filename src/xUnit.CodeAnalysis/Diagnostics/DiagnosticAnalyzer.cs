using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using xUnit.CodeAnalysis.Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;

namespace xUnit.CodeAnalysis.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public partial class XUnitCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string FactAttributeTypeFullName = typeof(FactAttribute).FullName;
        private static readonly string TheoryAttributeTypeFullName = typeof(TheoryAttribute).FullName;
        private static readonly string InlineDataAttributeTypeFullName = typeof(InlineDataAttribute).FullName;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(
                FactWithParametersRule, 
                TheoryWithoutParametersRule,
                MultipleFactDerivedAttributesRule,
                InlineDataWithoutTheoryRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol) context.Symbol;
            var factSymbol = context.Compilation.GetTypeByMetadataName(FactAttributeTypeFullName);
            var theorySymbol = context.Compilation.GetTypeByMetadataName(TheoryAttributeTypeFullName);
            var inlineDataSymbol = context.Compilation.GetTypeByMetadataName(InlineDataAttributeTypeFullName);

            var factDerivedAttributes = methodSymbol
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(factSymbol))
                .ToImmutableArray();

            if (!factDerivedAttributes.Any() && InlineDataWithoutTheory(factDerivedAttributes, theorySymbol, inlineDataSymbol, methodSymbol))
                context.ReportDiagnostic(CreateInlineDataWithoutTheoryDiagnostic(methodSymbol));

            if (!factDerivedAttributes.Any())
                return;

            if (MultipleFactDerivedAttributes(factDerivedAttributes))
                context.ReportDiagnostic(CreateMultipleFactDerivedAttributesDiagnostic(methodSymbol));
            else if (FactWithParameters(factDerivedAttributes, factSymbol, methodSymbol))
                context.ReportDiagnostic(CreateFactWithParametersDiagnostic(methodSymbol));
            else if (TheoryWithoutParameters(factDerivedAttributes, theorySymbol, methodSymbol))
                context.ReportDiagnostic(CreateTheoryWithoutParametersDiagnostic(methodSymbol));
        }
    }
}
