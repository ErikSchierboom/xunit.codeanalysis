using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class ValidTestMethodTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("[Fact]")]
        [InlineData("[Fact][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\")][Fact]")]
        [InlineData("[Before(\"before\",\"b\")][Fact][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\"), Fact, Trait(\"after\",\"a\")]")]
        [InlineData("[Fact][Before(\"before\",\"b\")][Trait(\"after\",\"a\")]")]
        [InlineData("[Fact, Before(\"before\",\"b\")][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\")][Trait(\"after\",\"a\")][Fact]")]
        [InlineData("[Before(\"before\",\"b\"), Trait(\"after\",\"a\"), Fact]")]
        public void NoDiagnosticsForValidFactBasedTestMethod(string attributes) => VerifyCSharpDiagnostic(CreateTestClass(attributes, ""));

        [Theory]
        [InlineData("[Theory][InlineData(true)]")]
        [InlineData("[Theory][InlineData(true)][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\")][Theory][InlineData(true)]")]
        [InlineData("[Before(\"before\",\"b\")][Theory][InlineData(true)][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\"), Theory, Trait(\"after\",\"a\")][InlineData(true)]")]
        [InlineData("[Theory][Before(\"before\",\"b\"), InlineData(true)][Trait(\"after\",\"a\")]")]
        [InlineData("[Theory, InlineData(true), Before(\"before\",\"b\")][Trait(\"after\",\"a\")]")]
        [InlineData("[Before(\"before\",\"b\")][Trait(\"after\",\"a\")][Theory][InlineData(true)]")]
        [InlineData("[Before(\"before\",\"b\"), Trait(\"after\",\"a\"), Theory, InlineData(true)]")]
        public void NoDiagnosticsForValidTheoryBasedTestMethod(string attributes) => VerifyCSharpDiagnostic(CreateTestClass(attributes, "bool expected"));
    }
}