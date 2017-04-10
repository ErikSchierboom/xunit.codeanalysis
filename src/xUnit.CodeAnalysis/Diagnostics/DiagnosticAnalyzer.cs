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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(
                FactWithParametersRule, 
                MultipleFactDerivedAttributesRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol) context.Symbol;
            var typeByMetadataName = context.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);

            var factDerivedAttributes = methodSymbol
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(typeByMetadataName))
                .ToImmutableArray();

            if (!factDerivedAttributes.Any())
                return;

            if (factDerivedAttributes.Length > 1)
                context.ReportDiagnostic(CreateMultipleFactDerivedAttributesDiagnostic(methodSymbol));
            else if (methodSymbol.Parameters.Any())
                context.ReportDiagnostic(CreateFactWithParametersDiagnostic(methodSymbol));
        }
    }
}
