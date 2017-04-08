using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace xUnit.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class XUnitCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        public const string FactWithParametersDiagnosticId = "FactWithParameters";

        private static readonly DiagnosticDescriptor FactWithParametersRule = new DiagnosticDescriptor(
            id: FactWithParametersDiagnosticId, 
            title: "[Fact] methods with parameters", 
            messageFormat: "[Fact] methods are not allowed to have parameters", 
            category: "xUnit.Usage", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true, 
            description: "[Fact] methods should not have parameters."
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(FactWithParametersRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            var typeByMetadataName = context.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            
            var factAttributes = methodSymbol.GetAttributes().Where(a => a.AttributeClass.Name == typeByMetadataName.Name).ToImmutableArray();

            if (factAttributes.Any())
            {
                if (methodSymbol.Parameters.Any())
                {
                    var diagnostic = Diagnostic.Create(FactWithParametersRule, methodSymbol.Locations[0], methodSymbol.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
            else
            {
                
            }

            //// Find just those named type symbols with names containing lowercase letters.
            //if (methodSymbol.Name.ToCharArray().Any(char.IsLower))
            //{
            //    // For all such symbols, produce a diagnostic.
            //    var diagnostic = Diagnostic.Create(FactWithParametersRule, methodSymbol.Locations[0], methodSymbol.Name);

            //    context.ReportDiagnostic(diagnostic);
            //}
        }
    }
}
