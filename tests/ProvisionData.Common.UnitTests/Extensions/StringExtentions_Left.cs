namespace ProvisionData.Extensions
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class StringExtentions_Left
    {
        [Theory]
        [InlineData("", 0, "")]
        [InlineData("", 5, "")]
        [InlineData("  ", 0, "")]
        [InlineData("  ", 5, "")]
        [InlineData("Hello", 0, "")]
        [InlineData("Hello", 1, "H")]
        [InlineData("Hello", 5, "Hello")]
        [InlineData("Hello", 6, "Hello")]
        public void Should_return_the_correct_result(String input, Int32 length, String expected)
           => input.Left(length).Should().Be(expected);
    }
}
