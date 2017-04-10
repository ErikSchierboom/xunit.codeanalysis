using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using xUnit.CodeAnalysis.Diagnostics;

namespace xUnit.CodeAnalysis.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XUnitCodeAnalysisCodeFixProvider)), Shared]
    public partial class XUnitCodeAnalysisCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create(
                XUnitCodeAnalysisAnalyzer.FactWithParametersDiagnosticId,
                XUnitCodeAnalysisAnalyzer.TheoryWithoutParametersDiagnosticId,
                XUnitCodeAnalysisAnalyzer.MultipleFactDerivedAttributesDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var methodDeclaration = await GetMethodDeclarationSyntax(context, diagnostic);

            if (diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.FactWithParametersDiagnosticId)
                context.RegisterCodeFix(CreateReplaceFactWithTheoryCodeAction(context, methodDeclaration), diagnostic);
            else if (diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.TheoryWithoutParametersDiagnosticId)
                context.RegisterCodeFix(CreateReplaceTheoryWithFactCodeAction(context, methodDeclaration), diagnostic);
            else if (diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.MultipleFactDerivedAttributesDiagnosticId)
                context.RegisterCodeFix(CreateMultipleFactDerivedAttributesCodeAction(context, methodDeclaration), diagnostic);
        }

        private static async Task<MethodDeclarationSyntax> GetMethodDeclarationSyntax(CodeFixContext context, Diagnostic diagnostic)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            return root.FindToken(diagnostic.Location.SourceSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .First();
        }
    }
}