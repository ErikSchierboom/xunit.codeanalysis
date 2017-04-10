using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class ValidTests : DiagnosticVerifier
    {
        [Fact]
        public void NoDiagnosticsForValidFactTestMethod()
        {
            const string test = @"
    using System;
    using Xunit;
    
    public class Tests
    {
        [Fact]
        public void Fact()
        {
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void NoDiagnosticsForValidTheoryTestMethod()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Theory]
        [InlineData(true)]
        public void Theory(bool expected)
        {
        }
    }";

            VerifyCSharpDiagnostic(test);
        }
    }
}