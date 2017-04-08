using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace xUnit.CodeAnalysis
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string FactWithParametersCodeFixTitle = "Remove parameters";

        private static CodeAction CreateFactWithParametersCodeAction(CodeFixContext context, MethodDeclarationSyntax declaration) 
            => CodeAction.Create(
                title: FactWithParametersCodeFixTitle,
                createChangedDocument: c => RemoveParametersFromTestMethodWithFactAndParameters(context.Document, declaration, c),
                equivalenceKey: FactWithParametersCodeFixTitle);

        private static async Task<Document> RemoveParametersFromTestMethodWithFactAndParameters(
            Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var parameterListWithoutParameters = methodDeclaration.ParameterList.WithParameters(new SeparatedSyntaxList<ParameterSyntax>());
            var methodDeclarationWithoutParameters = methodDeclaration.WithParameterList(parameterListWithoutParameters);

            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(methodDeclaration, methodDeclarationWithoutParameters);

            return document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
