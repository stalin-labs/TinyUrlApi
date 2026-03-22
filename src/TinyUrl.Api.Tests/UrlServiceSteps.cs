using FluentAssertions;
using Moq;
using TinyUrl.Api.Core;

namespace TinyUrl.Api.Tests;

internal sealed class UrlServiceSteps
{
    public static readonly Uri TinyUrlBaseAddress = new("https://tiny.ul/");

    private readonly Mock<IRepository> _repository = new();

    private object? _result;
    private UrlService _target = null!;

    public UrlServiceSteps GivenIHaveUrlService()
    {
        _target = new UrlService(_repository.Object, TinyUrlBaseAddress);
        return this;
    }

    public UrlServiceSteps GivenISetupRepository(Action<Mock<IRepository>> setup)
    {
        setup(_repository);
        return this;
    }

    public async Task<UrlServiceSteps> WhenICallIsTinyUrlAsync(Uri uri)
    {
        _result = _target.IsTinyUrl(uri);
        return this;
    }

    public async Task<UrlServiceSteps> WhenICallGetTinyUrlAsync(Uri uri)
    {
        _result = await _target.GetTinyUrlAsync(uri);
        return this;
    }

    public async Task<UrlServiceSteps> WhenICallGetOriginalUrlAsync(Uri uri)
    {
        _result = await _target.GetOriginalUrlAsync(uri);
        return this;
    }

    public UrlServiceSteps ThenTheExpectedResultShouldBe<T>(T value)
    {
        _result.Should().BeOfType<T>();
        _result.Should().Be(value);
        return this;
    }

    public UrlServiceSteps ThenTheRepositoryShouldBeVerified()
    {
        _repository.Verify();
        return this;
    }
}
