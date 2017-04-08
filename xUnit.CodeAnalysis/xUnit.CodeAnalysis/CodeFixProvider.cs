using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace xUnit.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XUnitCodeAnalysisCodeFixProvider)), Shared]
    public class XUnitCodeAnalysisCodeFixProvider : CodeFixProvider
    {
        private const string FactWithParametersCodeFixTitle = "Remove parameters";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(XUnitCodeAnalysisAnalyzer.FactWithParametersDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: FactWithParametersCodeFixTitle,
                    createChangedDocument: c => RemoveParameters(context.Document, declaration, c),
                    equivalenceKey: FactWithParametersCodeFixTitle),
                diagnostic);
        }

        private async Task<Document> RemoveParameters(Document document, MethodDeclarationSyntax methodWithParameters, CancellationToken cancellationToken)
        {
            var parameterListWithoutParametrs = methodWithParameters.ParameterList.WithParameters(new SeparatedSyntaxList<ParameterSyntax>());
            var methodWithoutParameters = methodWithParameters.WithParameterList(parameterListWithoutParametrs);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var updatedRoot = root.ReplaceNode(methodWithParameters, methodWithoutParameters);

            return document.WithSyntaxRoot(updatedRoot);
        }
    }
}