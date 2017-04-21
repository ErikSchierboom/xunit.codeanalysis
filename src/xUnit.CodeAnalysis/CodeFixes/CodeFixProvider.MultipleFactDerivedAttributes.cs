using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using xUnit.CodeAnalysis.Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;

namespace xUnit.CodeAnalysis.CodeFixes
{
    public partial class XUnitCodeAnalysisCodeFixProvider
    {
        private const string MultipleFactDerivedAttributesCodeFixTitle = "Remove duplicate [Fact]-derived attributes";

        private CodeAction CreateMultipleFactDerivedAttributesCodeAction() 
            => CodeAction.Create(
                title: MultipleFactDerivedAttributesCodeFixTitle,
                createChangedDocument: RemoveDuplicateFactDerivedAttributes,
                equivalenceKey: MultipleFactDerivedAttributesCodeFixTitle);

        private async Task<Document> RemoveDuplicateFactDerivedAttributes(CancellationToken cancellationToken)
        {
            var semanticModel = await _context.Document.GetSemanticModelAsync(cancellationToken);
            var syntaxRoot = await _context.Document.GetSyntaxRootAsync(cancellationToken);
            
            var factSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            var symbolInfo = (IMethodSymbol)semanticModel.GetDeclaredSymbol(_methodDeclaration, cancellationToken);

            var factDerivedAttributes = symbolInfo
                .GetAttributes()
                .Where(a => a.AttributeClass.EqualsOrInheritsFrom(factSymbol))
                .ToImmutableArray();

            var updatedMethodDeclaration = _methodDeclaration.RemoveNodes(
                factDerivedAttributes.Skip(1).Select(a => a.ApplicationSyntaxReference.GetSyntax(cancellationToken)),
                SyntaxRemoveOptions.KeepTrailingTrivia);

            updatedMethodDeclaration = updatedMethodDeclaration.RemoveNodes(
                updatedMethodDeclaration.AttributeLists.Where(a => !a.Attributes.Any()),
                SyntaxRemoveOptions.KeepTrailingTrivia);

            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(_methodDeclaration, updatedMethodDeclaration);
            return _context.Document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
