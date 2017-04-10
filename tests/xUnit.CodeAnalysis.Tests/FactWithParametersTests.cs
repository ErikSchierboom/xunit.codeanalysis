using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class FactWithParametersTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[Fact]", "int expected")]
        [InlineData("[Fact]", "string input, bool valid, int expected")]
        [InlineData("[Fact]", "string input, bool valid, int expected")]
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
        [InlineData("[Fact]", "string input, bool valid, int expected", "[Theory]")]
        public void CodeFixForFactWithParameters(string attributes, string parameters, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, parameters);
            var expectedTestClass = CreateTestClass(expectedAttributes, parameters);
            
            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}