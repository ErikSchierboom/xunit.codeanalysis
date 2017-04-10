using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class ValidTestMethodTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("[Fact]")]
        [InlineData("[Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\")]")]
        [InlineData("[Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Fact]")]
        public void NoDiagnosticsForValidFactBasedTestMethod(string attributes) => VerifyCSharpDiagnostic(CreateTestClass(attributes, ""));

        [Theory]
        [InlineData("[Theory][InlineData(true)]")]
        [InlineData("[Theory][InlineData(true)][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][InlineData(true)]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][InlineData(true)][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")][InlineData(true)]")]
        [InlineData("[Theory][a(\"a\",\"b\"), InlineData(true)][Trait(\"c\",\"d\")]")]
        [InlineData("[Theory, InlineData(true), Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"d\")][Trait(\"c\",\"d\")][Theory][InlineData(true)]")]
        [InlineData("[Trait(\"a\",\"d\"), Trait(\"c\",\"d\"), Theory, InlineData(true)]")]
        public void NoDiagnosticsForValidTheoryBasedTestMethod(string attributes) => VerifyCSharpDiagnostic(CreateTestClass(attributes, "bool expected"));
    }
}