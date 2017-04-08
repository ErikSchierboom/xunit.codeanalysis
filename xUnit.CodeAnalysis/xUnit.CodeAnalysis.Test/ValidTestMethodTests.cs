using Microsoft.VisualStudio.TestTools.UnitTesting;
using xUnit.CodeAnalysis.Test.Helpers;

namespace xUnit.CodeAnalysis.Test
{
    [TestClass]
    public class ValidTestMethodTests : DiagnosticVerifier
    {
        [TestMethod]
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

        [TestMethod]
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