using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class TheoryWithoutParametersTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[Theory]")]
        [InlineData("[Theory][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]")]
        public void DiagnosticForTheoryWithoutParameters(string attributes)
        {
            var testClass = CreateTestClass(attributes, "");

            var expected = new DiagnosticResult
            {
                Id = "TheoryWithoutParameters",
                Message = "[Theory] methods must have one or more parameters",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 21)
                    }
            };

            VerifyCSharpDiagnostic(testClass, expected);
        }

        [Theory]
        [InlineData("[Theory]", "[Fact]")]
        [InlineData("[Theory][Trait(\"c\",\"d\")]", "[Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory]", "[Trait(\"a\",\"b\")][Fact]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]", "[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]", "[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\")]")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "[Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "[Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]", "[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]", "[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Fact]")]
        public void CodeFixForTheoryWithoutParameters(string attributes, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, "");
            var expectedTestClass = CreateTestClass(expectedAttributes, "");
            
            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}