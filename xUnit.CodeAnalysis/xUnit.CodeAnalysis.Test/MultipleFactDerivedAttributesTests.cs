using Microsoft.CodeAnalysis;
using xUnit.CodeAnalysis.Test.Helpers;
using Xunit;

namespace xUnit.CodeAnalysis.Test
{
    public class MultipleFactDerivedAttributesTests : CodeFixVerifier
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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