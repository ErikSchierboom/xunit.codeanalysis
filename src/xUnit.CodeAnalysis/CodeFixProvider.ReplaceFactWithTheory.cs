using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace xUnit.CodeAnalysis
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string ReplaceFactWithTheoryCodeFixTitle = "Replace [Fact] with [Theory]";

        private static CodeAction CreateReplaceFactWithTheoryCodeAction(CodeFixContext context, MethodDeclarationSyntax declaration) 
            => CodeAction.Create(
                title: ReplaceFactWithTheoryCodeFixTitle,
                createChangedDocument: c => ReplaceFactWithTheory(context.Document, declaration, c),
                equivalenceKey: ReplaceFactWithTheoryCodeFixTitle);

        private static async Task<Document> ReplaceFactWithTheory(
            Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var theoryAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Theory"));

            var factSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            var symbolInfo = (IMethodSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel, methodDeclaration, cancellationToken);

            foreach (var factAttribute in symbolInfo.GetAttributes().Where(a => a.AttributeClass.Equals(factSymbol)))
                syntaxRoot = syntaxRoot.ReplaceNode(factAttribute.ApplicationSyntaxReference.GetSyntax(cancellationToken), theoryAttribute);

            return document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
