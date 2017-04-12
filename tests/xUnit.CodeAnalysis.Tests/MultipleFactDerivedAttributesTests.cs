using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class MultipleFactDerivedAttributesTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[Fact][Fact]", "int expected")]
        [InlineData("[Fact][Trait(\"c\",\"d\")][Fact]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Fact]", "string input, bool valid, int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected")]
        [InlineData("[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\"), Theory]", "int expected")]
        [InlineData("[Fact][Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "string input, bool valid")]
        [InlineData("[Theory, Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]", "string input, bool valid")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected")]
        public void DiagnosticForMultipleFactDerivedAttributes(string attributes, string parameters)
        {
            var testClass = CreateTestClass(attributes, parameters);

            var expected = new DiagnosticResult
            {
                Id = "MultipleFactDerivedAttributes",
                Message = "Method 'Test' has multiple [Fact]-derived attributes",
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
        [InlineData("[Fact][Fact]", "int expected", "[Fact]")]
        [InlineData("[Fact][Trait(\"c\",\"d\")][Fact]", "int expected", "[Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Fact]", "", "[Trait(\"a\",\"b\")][Fact]")]
        [InlineData("[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected", "[Trait(\"a\",\"b\")][Fact][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\"), Theory]", "int expected", "[Trait(\"a\",\"b\"), Fact, Trait(\"c\",\"d\")]")]
        [InlineData("[Fact][Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "", "[Fact][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Theory, Fact, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "", "[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Fact]", "", "[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\"), Fact]", "string input, bool valid, int expected", "[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]")]
        public void CodeFixForMultipleFactDerivedAttributes(string attributes, string parameters, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, parameters);
            var expectedTestClass = CreateTestClass(expectedAttributes, parameters);

            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}