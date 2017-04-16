using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string AddTheoryCodeFixTitle = "Add [Theory] attribute";

        private CodeAction CreateAddTheoryCodeAction()
            => CodeAction.Create(
                title: AddTheoryCodeFixTitle,
                createChangedDocument: AddTheoryAttribute,
                equivalenceKey: AddTheoryCodeFixTitle);

        private async Task<Document> AddTheoryAttribute(CancellationToken cancellationToken)
        {
            var syntaxRoot = await _context.Document.GetSyntaxRootAsync(cancellationToken);
            
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Theory"));
            var attributeList = SyntaxFactory.AttributeList()
                .AddAttributes(attribute)
                .WithLeadingTrivia(_methodDeclaration.GetLeadingTrivia());

            var updatedMethodDeclaration = _methodDeclaration.WithAttributeLists(
                _methodDeclaration.AttributeLists.Insert(0, attributeList));

            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(_methodDeclaration, updatedMethodDeclaration);
            return _context.Document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
