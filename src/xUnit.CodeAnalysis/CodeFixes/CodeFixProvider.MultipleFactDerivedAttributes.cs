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
            
            var factSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(FactAttribute).FullName);
            var theorySymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(TheoryAttribute).FullName);

            var symbolInfo = (IMethodSymbol)semanticModel.GetDeclaredSymbol(methodDeclaration);
            var factDerivedAttributes = symbolInfo
                .GetAttributes()
                .Where(a => a.AttributeClass.InheritsFromOrEquals(factSymbol))
                .ToImmutableArray();

            SyntaxList<AttributeListSyntax> attributeLists = methodDeclaration.AttributeLists;

            if (methodDeclaration.ParameterList.Parameters.Count > 0)
            {
                var theoryAttribute = factDerivedAttributes.FirstOrDefault(f => f.AttributeClass.InheritsFromOrEquals(theorySymbol));
                if (theoryAttribute != null)
                {
                    //attributeLists = new SyntaxList<AttributeListSyntax> { theoryAttribute.ApplicationSyntaxReference.GetSyntax(cancellationToken) };
                }
                else
                {
                    
                }
            }

            var methodDeclarationWithoutParameters = methodDeclaration.WithAttributeLists(attributeLists);

            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var updatedSyntaxRoot = syntaxRoot.ReplaceNode(methodDeclaration, methodDeclarationWithoutParameters);

            return document.WithSyntaxRoot(updatedSyntaxRoot);
        }
    }
}
