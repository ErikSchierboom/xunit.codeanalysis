using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string AddTheoryCodeFixTitle = "Add [Theory] attribute";

        private static CodeAction CreateAddTheoryCodeAction(CodeFixContext context, MethodDeclarationSyntax declaration)
            => CodeAction.Create(
                title: AddTheoryCodeFixTitle,
                createChangedDocument: c => AddTheoryAttribute(context.Document, declaration, c),
                equivalenceKey: AddTheoryCodeFixTitle);

        private static async Task<Document> AddTheoryAttribute(
            Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Theory"));
            var attributeList = SyntaxFactory.AttributeList()
                .AddAttributes(attribute)
                .WithLeadingTrivia(methodDeclaration.GetLeadingTrivia());

            var updatedMethodDeclaration = methodDeclaration.WithAttributeLists(
                methodDeclaration.AttributeLists.Insert(0, attributeList));

            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(methodDeclaration, updatedMethodDeclaration);
            return document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
