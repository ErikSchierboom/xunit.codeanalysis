using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class TheoryWithoutDataTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[Theory]", "int expected")]
        [InlineData("[Theory][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Theory]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]", "int expected")]
        public void DiagnosticForTheoryWithoutData(string attributes, string parameters)
        {
            var testClass = CreateTestClass(attributes, "int expected");

            var expected = new DiagnosticResult
            {
                Id = "TheoryWithoutData",
                Message = "[Theory] methods must have one or more [Data]-derived attributes",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 21)
                    }
            };

            VerifyCSharpDiagnostic(testClass, expected);
        }

        [Theory(Skip = "Due to a Roslyn bug, this doesn't compile in the test project")]
        [InlineData("[Theory]", "[Theory]\n[InlineData(TODO)]")]
        [InlineData("[Theory][Trait(\"c\",\"d\")]", "[Theory][Trait(\"c\",\"d\")]\n[InlineData(TODO)]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory]", "[Trait(\"a\",\"b\")][Theory]\n[InlineData(TODO)]")]
        [InlineData("[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]", "[Trait(\"a\",\"b\")][Theory][Trait(\"c\",\"d\")]\n[InlineData(TODO)]")]
        [InlineData("[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]", "[Trait(\"a\",\"b\"), Theory, Trait(\"c\",\"d\")]\n[InlineData(TODO)]")]
        [InlineData("[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]\n[InlineData(TODO)]")]
        [InlineData("[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "[Theory, Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]\n[InlineData(TODO)]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]", "[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][Theory]\n[InlineData(TODO)]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]", "[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), Theory]\n[InlineData(TODO)]")]
        public void CodeFixForTheoryWithoutData(string attributes, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, "int expected");
            var expectedTestClass = CreateTestClass(expectedAttributes, "int expected");
            
            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}