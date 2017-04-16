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
        private CodeFixContext _context;
        private Diagnostic _diagnostic;
        private MethodDeclarationSyntax _methodDeclaration;
        
        public sealed override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create(
                XUnitCodeAnalysisAnalyzer.FactWithParametersDiagnosticId,
                XUnitCodeAnalysisAnalyzer.TheoryWithoutParametersDiagnosticId,
                XUnitCodeAnalysisAnalyzer.MultipleFactDerivedAttributesDiagnosticId,
                XUnitCodeAnalysisAnalyzer.InlineDataWithoutTheoryDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            _context = context;
            _diagnostic = context.Diagnostics.First();
            _methodDeclaration = await GetMethodDeclarationSyntax();

            if (_diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.FactWithParametersDiagnosticId)
                context.RegisterCodeFix(CreateReplaceFactWithTheoryCodeAction(), _diagnostic);
            else if (_diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.TheoryWithoutParametersDiagnosticId)
                context.RegisterCodeFix(CreateReplaceTheoryWithFactCodeAction(), _diagnostic);
            else if (_diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.MultipleFactDerivedAttributesDiagnosticId)
                context.RegisterCodeFix(CreateMultipleFactDerivedAttributesCodeAction(), _diagnostic);
            else if (_diagnostic.Descriptor.Id == XUnitCodeAnalysisAnalyzer.InlineDataWithoutTheoryDiagnosticId)
                context.RegisterCodeFix(CreateAddTheoryCodeAction(), _diagnostic);
        }

        private async Task<MethodDeclarationSyntax> GetMethodDeclarationSyntax()
        {
            var root = await _context.Document.GetSyntaxRootAsync(_context.CancellationToken).ConfigureAwait(false);
            return root.FindToken(_diagnostic.Location.SourceSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .First();
        }
    }
}