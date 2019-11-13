namespace ProvisionData.UnitTests.Specifications
{
    using System;
    using System.Linq.Expressions;
    using FluentAssertions;
    using ProvisionData.Specifications;
    using Xunit;

    public class Specifications
    {
        internal class IsEven : AbstractSpecification<Int32>
        {
            public override Expression<Func<Int32, Boolean>> SpecificationExpression
                => n => (n % 2) == 0;
        }

        internal class IsMultiple : AbstractSpecification<Int32>
        {
            private readonly Int32 _factor;

            public IsMultiple(Int32 factor) => _factor = factor;

            public override Expression<Func<Int32, Boolean>> SpecificationExpression
                => n => (n % _factor) == 0;
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(5, false)]
        [InlineData(9, false)]
        [InlineData(10, true)]
        [InlineData(11, false)]
        [InlineData(15, false)]
        [InlineData(20, true)]
        public void Can_be_combined(Int32 input, Boolean expected)
        {
            ISpecification<Int32> isTen = new IsEven().And(new IsMultiple(5));

            isTen.IsSatisfiedBy(input).Should().Be(expected);
        }
    }
}
