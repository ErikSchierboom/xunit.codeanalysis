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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(
                FactWithParametersRule, 
                MultipleFactDerivedAttributesRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol) context.Symbol;
            var factAttribute = context.Compilation.GetTypeByMetadataName(FactAttributeTypeFullName);

            var factDerivedAttributes = methodSymbol
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(factAttribute))
                .ToImmutableArray();

            if (!factDerivedAttributes.Any())
                return;

            if (MultipleFactDerivedAttributes(factDerivedAttributes))
                context.ReportDiagnostic(CreateMultipleFactDerivedAttributesDiagnostic(methodSymbol));
            else if (FactWithParameters(factDerivedAttributes, factAttribute, methodSymbol))
                context.ReportDiagnostic(CreateFactWithParametersDiagnostic(methodSymbol));
        }
    }
}
