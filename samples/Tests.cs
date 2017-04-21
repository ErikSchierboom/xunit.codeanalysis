using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

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

        public class CustomData : DataAttribute
        {
            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                return new object[][]
                {
                    new object[] {1},
                    new object[] {2},
                };
            }
        }

        [Theory]
        [CustomData]
        public void TheoryWithoutData(int expected)
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
