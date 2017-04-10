using System;
using Xunit;

namespace xUnit.CodeAnalysis.Sample
{
    public class Tests
    {
        [Fact]
        public void FactWithParameters(int expected)
        {
        }

        [Fact]
        public void FactWithMultipleParameters(string input, bool valid, int expected)
        {
        }


        [Trait("Before", "x"), Fact, Trait("After", "y")]
        public void FactInParameterListWithParameters(int expected)
        {
        }

        [Trait("Before", "x"), Fact, Trait("After", "y")]
        public void FactInParameterListWithMultipleParameters(string input, bool valid, int expected)
        {
        }

        [Theory]
        public void TheoryWithoutData()
        {
        }

        [Theory]
        [InlineData(1)]
        public void TheoryWithoutParameters()
        {
        }

        [InlineData(1)]
        public void InlineDataWithoutTheory()
        {
        }

        [Fact]
        [Theory]
        public void FactAndTheory()
        {
        }

        [Theory]
        [InlineData]
        public void TheoryWithEmptyInlineData()
        {
        }

        [Theory]
        [InlineData(1, true)]
        public void TheoryWithTooFewParameters(int i)
        {
        }

        [Theory]
        [InlineData(1, true)]
        public void TheoryWithTooManyParameters(int i, bool b, float f)
        {
        }
        
        [Theory]
        [InlineData(1)]
        public void TheoryWithNonMatchingParametType(DateTime d)
        {
        }

        public void AssertWithoutFact()
        {
            Assert.True(true);
        }
        
        public void AssertWithoutTheory(int i, string s)
        {
            Assert.True(true);
        }
    }
}
