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
        private const string ReplaceTheoryWithFactCodeFixTitle = "Replace [Theory] with [Fact]";

        private CodeAction CreateReplaceTheoryWithFactCodeAction() 
            => CodeAction.Create(
                title: ReplaceTheoryWithFactCodeFixTitle,
                createChangedDocument: ReplaceTheoryWithFact,
                equivalenceKey: ReplaceTheoryWithFactCodeFixTitle);

        private async Task<Document> ReplaceTheoryWithFact(CancellationToken cancellationToken)
        {
            var semanticModel = await _context.Document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await _context.Document.GetSyntaxRootAsync(cancellationToken);
            var factAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Fact"));

            var theorySymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(TheoryAttribute).FullName);
            var symbolInfo = semanticModel.GetDeclaredSymbol(_methodDeclaration, cancellationToken);

            foreach (var theoryAttribute in symbolInfo.GetAttributes().Where(a => a.AttributeClass.Equals(theorySymbol)))
                syntaxRoot = syntaxRoot.ReplaceNode(theoryAttribute.ApplicationSyntaxReference.GetSyntax(cancellationToken), factAttribute);

            return _context.Document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
