using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string ReplaceTheoryWithFactCodeFixTitle = "Replace [Theory] with [Fact]";

        private static CodeAction CreateReplaceTheoryWithFactCodeAction(CodeFixContext context, MethodDeclarationSyntax declaration) 
            => CodeAction.Create(
                title: ReplaceTheoryWithFactCodeFixTitle,
                createChangedDocument: c => ReplaceTheoryWithFact(context.Document, declaration, c),
                equivalenceKey: ReplaceTheoryWithFactCodeFixTitle);

        private static async Task<Document> ReplaceTheoryWithFact(
            Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var factAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("Fact"));

            var theorySymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(TheoryAttribute).FullName);
            var symbolInfo = (IMethodSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel, methodDeclaration, cancellationToken);

            foreach (var theoryAttribute in symbolInfo.GetAttributes().Where(a => a.AttributeClass.Equals(theorySymbol)))
                syntaxRoot = syntaxRoot.ReplaceNode(theoryAttribute.ApplicationSyntaxReference.GetSyntax(cancellationToken), factAttribute);

            return document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
