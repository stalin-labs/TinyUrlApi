using FluentAssertions;
using TinyUrl.Api.Helpers;
using Xunit;

namespace TinyUrl.Api.Tests;

public sealed class ShortUrlHelpersTests
{
    [Theory]
    [InlineData(1, "MQ==")]
    [InlineData(1000, "MTAwMA==")]
    public void ShortUrlHelper_Encode_ReturnsExpected(int data, string expected)
    {
        var result = ShortUrlHelpers.Encode(data);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("MQ==", 1)]
    [InlineData("MTAwMA==", 1000)]
    public void ShortUrlHelper_Decode_ReturnsExpected(string encodedData, int expected)
    {
        var result = ShortUrlHelpers.TryDecode(encodedData, out int actual);
        result.Should().BeTrue();
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("$MQ==!~")]
    [InlineData("#MTAwMA==!~")]
    public void ShortUrlHelper_DecodeInvalidData_ReturnsFalse(string encodedData)
    {
        ShortUrlHelpers.TryDecode(encodedData, out int _).Should().BeFalse();
    }
}
