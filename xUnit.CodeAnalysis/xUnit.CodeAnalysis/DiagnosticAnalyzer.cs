using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using xUnit.CodeAnalysis.Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;

namespace xUnit.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class XUnitCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        public const string FactWithParametersDiagnosticId = "FactWithParameters";
        public const string MultipleFactDerivedAttributesDiagnosticId = "MultipleFactDerivedAttributes";
        
        private static readonly DiagnosticDescriptor FactWithParametersRule = new DiagnosticDescriptor(
            id: FactWithParametersDiagnosticId, 
            title: "[Fact] method with parameters", 
            messageFormat: "[Fact] methods are not allowed to have parameters", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true, 
            description: "[Fact] methods should not have parameters."
        );

        private static readonly DiagnosticDescriptor MultipleFactDerivedAttributesRule = new DiagnosticDescriptor(
            id: MultipleFactDerivedAttributesDiagnosticId,
            title: "Test method with multiple [Fact]-derived attributes",
            messageFormat: "Method '{0}' has multiple [Fact]-derived attributes",
            category: "xUnit.Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Test methods should not have multiple [Fact]-derived attributes."
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(
                FactWithParametersRule, 
                MultipleFactDerivedAttributesRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var typeByMetadataName = context.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            
            var factDerivedAttributes = methodSymbol
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(typeByMetadataName))
                .ToImmutableArray();

            if (!factDerivedAttributes.Any())
                return;

            if (factDerivedAttributes.Length > 1)
                context.ReportDiagnostic(Diagnostic.Create(MultipleFactDerivedAttributesRule, methodSymbol.Locations[0], methodSymbol.Name));
            else if (methodSymbol.Parameters.Any())
                context.ReportDiagnostic(Diagnostic.Create(FactWithParametersRule, methodSymbol.Locations[0], methodSymbol.Name));
        }
    }
}
