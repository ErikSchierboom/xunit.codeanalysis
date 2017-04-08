using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xUnit.CodeAnalysis.Test.Helpers;

namespace xUnit.CodeAnalysis.Test
{
    [TestClass]
    public class MultipleFactDerivedAttributesTests : CodeFixVerifier
    {
        [TestMethod]
        public void DiagnosticForMultipleFactDerivedAttributes()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        [Theory]
        public void MultipleFactDerivedAttributes()
        {
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = "MultipleFactDerivedAttributes",
                Message = "Method 'MultipleFactDerivedAttributes' has multiple [Fact]-derived attributes",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CodeFixForMultipleFactDerivedAttributesOnMethodWithoutParameters()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        [Theory]
        public void MultipleFactDerivedAttributes()
        {
        }
    }";

            const string fixtest = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        public void MultipleFactDerivedAttributes()
        {
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void CodeFixForMultipleFactDerivedAttributesOnMethodWithParameters()
        {
            const string test = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Fact]
        [Theory]
        public void MultipleFactDerivedAttributes(int expected)
        {
        }
    }";

            const string fixtest = @"
    using System;
    using Xunit;

    public class Tests
    {
        [Theory]
        public void MultipleFactDerivedAttributes(int expected)
        {
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
    }
}