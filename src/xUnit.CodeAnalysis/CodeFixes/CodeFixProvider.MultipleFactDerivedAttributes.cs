using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using xUnit.CodeAnalysis.Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string MultipleFactDerivedAttributesCodeFixTitle = "Remove duplicate [Fact]-derived attributes";

        private static CodeAction CreateMultipleFactDerivedAttributesCodeAction(CodeFixContext context, MethodDeclarationSyntax declaration) 
            => CodeAction.Create(
                title: MultipleFactDerivedAttributesCodeFixTitle,
                createChangedDocument: c => RemoveDuplicateFactDerivedAttributes(context.Document, declaration, c),
                equivalenceKey: MultipleFactDerivedAttributesCodeFixTitle);

        private static async Task<Document> RemoveDuplicateFactDerivedAttributes(
            Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            
            var factSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            var symbolInfo = (IMethodSymbol)semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            var factDerivedAttributes = symbolInfo
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(factSymbol))
                .ToImmutableArray();

            var updatedMethodDeclaration = methodDeclaration.RemoveNodes(
                factDerivedAttributes.Skip(1).Select(a => a.ApplicationSyntaxReference.GetSyntax(cancellationToken)),
                SyntaxRemoveOptions.KeepTrailingTrivia);

            updatedMethodDeclaration = updatedMethodDeclaration.RemoveNodes(
                updatedMethodDeclaration.AttributeLists.Where(a => !a.Attributes.Any()),
                SyntaxRemoveOptions.KeepTrailingTrivia);

            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(methodDeclaration, updatedMethodDeclaration);
            return document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
