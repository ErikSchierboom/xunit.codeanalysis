using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string ReplaceFactWithTheoryCodeFixTitle = "Replace [Fact] with [Theory]";

        private CodeAction CreateReplaceFactWithTheoryCodeAction() 
            => CodeAction.Create(
                title: ReplaceFactWithTheoryCodeFixTitle,
                createChangedDocument: ReplaceFactWithTheory,
                equivalenceKey: ReplaceFactWithTheoryCodeFixTitle);

        private async Task<Document> ReplaceFactWithTheory(CancellationToken cancellationToken)
        {
            var semanticModel = await _context.Document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await _context.Document.GetSyntaxRootAsync(cancellationToken);
            var theoryAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Theory"));

            var factSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            var symbolInfo = semanticModel.GetDeclaredSymbol(_methodDeclaration, cancellationToken);

            foreach (var factAttribute in symbolInfo.GetAttributes().Where(a => a.AttributeClass.Equals(factSymbol)))
                syntaxRoot = syntaxRoot.ReplaceNode(factAttribute.ApplicationSyntaxReference.GetSyntax(cancellationToken), theoryAttribute);

            return _context.Document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
