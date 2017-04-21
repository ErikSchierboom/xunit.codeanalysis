using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class InlineDataWithoutTheoryTests : CodeFixVerifier
    {
        [Theory]
        [InlineData("[InlineData]", "int expected")]
        [InlineData("[InlineData(1)][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][InlineData(\"\",1,false)]", "string input, bool valid, int expected")]
        [InlineData("[Trait(\"a\",\"b\")][InlineData(\"\",1,false)][Trait(\"c\",\"d\")]", "string input, bool valid, int expected")]
        [InlineData("[InlineData(1)(\"a\",\"b\"), Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[InlineData(\"\",false)][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "string input, bool valid")]
        [InlineData("[InlineData(1), Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][InlineData(\"\",false)]", "string input, bool valid")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), InlineData(\"\",1,false)]", "string input, bool valid, int expected")]
        public void DiagnosticForInlineDataWithoutTheory(string attributes, string parameters)
        {
            var testClass = CreateTestClass(attributes, parameters);

            var expected = new DiagnosticResult
            {
                Id = "InlineDataWithoutTheory",
                Message = "[InlineData] should be accompanied by [Theory]",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 21)
                    }
            };

            VerifyCSharpDiagnostic(testClass, expected);
        }

        [Theory(Skip = "Due to a Roslyn bug, this doesn't compile in the test project")]
        [InlineData("[InlineData]", "int expected", "[Theory][InlineData]")]
        [InlineData("[InlineData(1)][Trait(\"c\",\"d\")]", "int expected", "[Theory][InlineData(1)][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][InlineData(\"\",1,false)]", "string input, bool valid, int expected", "[Theory][Trait(\"a\",\"b\")][InlineData(\"\",1,false)]")]
        [InlineData("[Trait(\"a\",\"b\")][InlineData(\"\",1,false)][Trait(\"c\",\"d\")]", "string input, bool valid, int expected", "[Theory][Trait(\"a\",\"b\")][InlineData(\"\",1,false)][Trait(\"c\",\"d\")]")]
        [InlineData("[InlineData(1, \"a\",\"b\"), Trait(\"c\",\"d\")]", "int expected", "[Theory][InlineData(1, \"a\",\"b\"), Trait(\"c\",\"d\")]")]
        [InlineData("[InlineData(\"\",false)][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "string input, bool valid", "[Theory][InlineData(\"\",false)][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[InlineData(1), Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]", "int expected", "[Theory][InlineData(1), Trait(\"a\",\"b\")][Trait(\"c\",\"d\")]")]
        [InlineData("[Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][InlineData(\"\",false)]", "string input, bool valid", "[Theory][Trait(\"a\",\"b\")][Trait(\"c\",\"d\")][InlineData(\"\",false)]")]
        [InlineData("[Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), InlineData(\"\",1,false)]", "string input, bool valid, int expected", "[Theory][Trait(\"a\",\"b\"), Trait(\"c\",\"d\"), InlineData(\"\",1,false)]")]
        public void CodeFixForInlineDataWithoutTheory(string attributes, string parameters, string expectedAttributes)
        {
            var testClass = CreateTestClass(attributes, parameters);
            var expectedTestClass = CreateTestClass(expectedAttributes, parameters);
            
            VerifyCSharpFix(testClass, expectedTestClass);
        }
    }
}