using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using xUnit.CodeAnalysis.Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;
using Xunit.Sdk;

namespace xUnit.CodeAnalysis.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public partial class XUnitCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string FactAttributeTypeFullName = typeof(FactAttribute).FullName;
        private static readonly string TheoryAttributeTypeFullName = typeof(TheoryAttribute).FullName;
        private static readonly string DataAttributeTypeFullName = typeof(DataAttribute).FullName;
        private static readonly string InlineDataAttributeTypeFullName = typeof(InlineDataAttribute).FullName;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(
                FactWithParametersRule, 
                TheoryWithoutParametersRule,
                TheoryWithoutDataRule,
                MultipleFactDerivedAttributesRule,
                InlineDataWithoutTheoryRule);

        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.RegisterCompilationStartAction(compilationStartContext =>
            {
                // By doing the symbol analysis here, we ensure that the symbols are only retrieved once per compilation
                var xUnitSymbolContext = new XUnitSymbolContext
                {
                    FactSymbol = compilationStartContext.Compilation.GetTypeByMetadataName(FactAttributeTypeFullName),
                    TheorySymbol = compilationStartContext.Compilation.GetTypeByMetadataName(TheoryAttributeTypeFullName),
                    DataSymbol = compilationStartContext.Compilation.GetTypeByMetadataName(DataAttributeTypeFullName),
                    InlineDataSymbol = compilationStartContext.Compilation.GetTypeByMetadataName(InlineDataAttributeTypeFullName)
                };

                compilationStartContext.RegisterSymbolAction(symbolContext =>
                {
                    xUnitSymbolContext.MethodSymbol = (IMethodSymbol) symbolContext.Symbol;
                    xUnitSymbolContext.FactDerivedAttributes = 
                        xUnitSymbolContext.MethodSymbol
                            .GetAttributes()
                            .Where(a => a.AttributeClass.EqualsOrInheritsFrom(xUnitSymbolContext.FactSymbol))
                            .ToImmutableArray();

                    if (!xUnitSymbolContext.FactDerivedAttributes.Any() && InlineDataWithoutTheory(xUnitSymbolContext))
                        symbolContext.ReportDiagnostic(CreateInlineDataWithoutTheoryDiagnostic(xUnitSymbolContext));

                    if (!xUnitSymbolContext.FactDerivedAttributes.Any())
                        return;

                    if (MultipleFactDerivedAttributes(xUnitSymbolContext))
                        symbolContext.ReportDiagnostic(CreateMultipleFactDerivedAttributesDiagnostic(xUnitSymbolContext));
                    else if (FactWithParameters(xUnitSymbolContext))
                        symbolContext.ReportDiagnostic(CreateFactWithParametersDiagnostic(xUnitSymbolContext));
                    else if (TheoryWithoutParameters(xUnitSymbolContext))
                        symbolContext.ReportDiagnostic(CreateTheoryWithoutParametersDiagnostic(xUnitSymbolContext));
                    else if (TheoryWithoutData(xUnitSymbolContext))
                        symbolContext.ReportDiagnostic(CreateTheoryWithoutDataDiagnostic(xUnitSymbolContext));
                }, SymbolKind.Method);
            });
        }

        private static Diagnostic CreateDiagnostic(DiagnosticDescriptor diagnostic, XUnitSymbolContext context)
            => Diagnostic.Create(diagnostic, context.MethodSymbol.Locations[0], context.MethodSymbol.Name);

        private class XUnitSymbolContext
        {
            public INamedTypeSymbol FactSymbol { get; set; }
            public INamedTypeSymbol TheorySymbol { get; set; }
            public INamedTypeSymbol DataSymbol { get; set; }
            public INamedTypeSymbol InlineDataSymbol { get; set; }
            public IMethodSymbol MethodSymbol { get; set; }
            public ImmutableArray<AttributeData> FactDerivedAttributes { get; set; }

            public bool HasFactAttribute => FactDerivedAttributes.Any(f => f.AttributeClass.Equals(FactSymbol));
            public bool HasTheoryAttribute => FactDerivedAttributes.Any(f => f.AttributeClass.Equals(TheorySymbol));
            public bool HasDataAttribute => MethodSymbol.GetAttributes().Any(f => f.AttributeClass.EqualsOrInheritsFrom(DataSymbol));
            public bool HasInlineDataAttribute => MethodSymbol.GetAttributes().Any(f => f.AttributeClass.Equals(InlineDataSymbol));
            public bool HasParameters => MethodSymbol.Parameters.Any();
        }
    }
}
