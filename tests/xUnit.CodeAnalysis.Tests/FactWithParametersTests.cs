using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class FactWithParametersTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[Fact]", "int expected")]
        [InlineData("[Fact][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Fact]", "string input, bool valid, int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\")]", "string input, bool valid, int expected")]
        [InlineData("[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "string input, bool valid")]
        [InlineData("[Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]", "string input, bool valid")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected")]
        public void DiagnosticForFactWithParameters(string attributes, string parameters)
        {
            var testClass = CreateTestClass(attributes, parameters);

            var expected = new DiagnosticResult
            {
                Id = "FactWithParameters",
                Message = "[Fact] methods are not allowed to have parameters",
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
        [InlineData("[Fact]", "int expected", "[Theory]")]
        [InlineData("[Fact][Trait(\"c\",\"d\")]", "int expected", "[Theory][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact]", "string input, bool valid, int expected", "[Trait(\"a\",\"b\")][Theory]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\")]", "string input, bool valid, int expected", "[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\")]", "int expected", "[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]")]
        [InlineData("[Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "string input, bool valid", "[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected", "[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]", "string input, bool valid", "[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected", "[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]")]
        public void CodeFixForFactWithParameters(string attributes, string parameters, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, parameters);
            var expectedTestClass = CreateTestClass(expectedAttributes, parameters);
            
            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}