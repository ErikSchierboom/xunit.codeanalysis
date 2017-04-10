using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xUnit.CodeAnalysis.Test.Helpers;

namespace xUnit.CodeAnalysis.Test
{
    [TestClass]
    public class FactWithParametersTests : CodeFixVerifier
    {
        [TestMethod]
        public void DiagnosticForFactWithOneParameter()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        public void FactWithParameters(int expected)
        {
        }
    }";

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

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CodeFixForFactWithOneParameter()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        public void FactWithParameters(int expected)
        {
        }
    }";

            const string fixtest = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Theory]
        public void FactWithParameters(int expected)
        {
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void DiagnosticForFactWithMultipleParameters()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        public void FactWithParameters(string input, bool valid, int expected)
        {
        }
    }";

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

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CodeFixForFactWithMultipleParameters()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        public void FactWithParameters(string input, bool valid, int expected)
        {
        }
    }";

            const string fixtest = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Theory]
        public void FactWithParameters(string input, bool valid, int expected)
        {
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
    }
}