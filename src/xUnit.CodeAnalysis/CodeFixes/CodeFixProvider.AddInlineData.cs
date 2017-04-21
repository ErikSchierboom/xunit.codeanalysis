using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string AddInlineDataCodeFixTitle = "Add [InlineData] attribute";

        private CodeAction CreateAddInlineDataCodeAction()
            => CodeAction.Create(
                title: AddInlineDataCodeFixTitle,
                createChangedDocument: AddInlineDataAttribute,
                equivalenceKey: AddInlineDataCodeFixTitle);

        private async Task<Document> AddInlineDataAttribute(CancellationToken cancellationToken)
        {
            var syntaxRoot = await _context.Document.GetSyntaxRootAsync(cancellationToken);
            
            var attribute = 
                SyntaxFactory.Attribute(
                    SyntaxFactory.ParseName("InlineData"),
                    SyntaxFactory.AttributeArgumentList(
                        SyntaxFactory.SeparatedList(
                            Enumerable.Repeat(
                                SyntaxFactory.AttributeArgument(SyntaxFactory.IdentifierName("TODO")), 
                                _methodDeclaration.ParameterList.Parameters.Count))));

            var attributeList = SyntaxFactory.AttributeList()
                .AddAttributes(attribute)
                .WithTrailingTrivia(_methodDeclaration.AttributeLists.Last().GetTrailingTrivia());

            var updatedMethodDeclaration = _methodDeclaration.WithAttributeLists(
                _methodDeclaration.AttributeLists.Add(attributeList));

            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(_methodDeclaration, updatedMethodDeclaration);
            return _context.Document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
