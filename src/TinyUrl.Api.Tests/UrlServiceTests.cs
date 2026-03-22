using Moq;
using Xunit;

namespace TinyUrl.Api.Tests;

public sealed class UrlServiceTests
{
    private const int RepositoryExistingRecordId = 1000;
    private const string RepositoryExistingRecordIdEncodedValue = "MTAwMA==";

    private readonly UrlServiceSteps _steps = new();

    public static IEnumerable<object[]> ValidTinyUrlTestData() =>
        new TheoryData<Uri>
        {
            { UrlServiceSteps.TinyUrlBaseAddress },
            { new UriBuilder(UrlServiceSteps.TinyUrlBaseAddress) { Path = "something" }.Uri }
        };

    [Fact]
    public async Task UrlService_NonTinyUrl_ReturnsFalse()
    {
        var nonTinyUrl = new Uri("https://nontinyUrl.com");

        (await _steps
          .GivenIHaveUrlService()
          .WhenICallIsTinyUrlAsync(nonTinyUrl))
          .ThenTheExpectedResultShouldBe(false);
    }

    [Theory]
    [MemberData(nameof(ValidTinyUrlTestData))]
    public async Task UrlService_TinyUrl_ReturnsTrue(Uri tinyUrl)
    {
        (await _steps
          .GivenIHaveUrlService()
          .WhenICallIsTinyUrlAsync(tinyUrl))
          .ThenTheExpectedResultShouldBe(true);
    }

    [Fact]
    public async Task UrlService_GetOriginalUrlForInvalidEncodedData_ReturnsEmpty()
    {
        var tinyUri = new UriBuilder(UrlServiceSteps.TinyUrlBaseAddress) { Path = "InvalidEncodedData" }.Uri;

        (await _steps
          .GivenIHaveUrlService()
          .WhenICallGetOriginalUrlAsync(tinyUri))
          .ThenTheExpectedResultShouldBe(string.Empty);
    }

    [Fact]
    public async Task UrlService_GetOriginalUrlForValidEncodedData_ReturnsExpected()
    {
        var tinyUri = new UriBuilder(UrlServiceSteps.TinyUrlBaseAddress)
        { Path = RepositoryExistingRecordIdEncodedValue }.Uri;

        (await _steps
          .GivenIHaveUrlService()
          .GivenISetupRepository(x =>
              x.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
               .ReturnsAsync("OriginalUrl")
               .Verifiable())
          .WhenICallGetOriginalUrlAsync(tinyUri))
          .ThenTheExpectedResultShouldBe("OriginalUrl")
          .ThenTheRepositoryShouldBeVerified();
    }

    [Fact]
    public async Task UrlService_GetTinyUrlForExistingUrl_ReturnsExpected()
    {
        var longUri = new Uri("http://longUrl");
        var expectedTinyUri = new UriBuilder(UrlServiceSteps.TinyUrlBaseAddress)
        { Path = RepositoryExistingRecordIdEncodedValue }.Uri.AbsoluteUri;

        (await _steps
          .GivenIHaveUrlService()
          .GivenISetupRepository(x =>
              x.Setup(s => s.GetByPathAsync(It.IsAny<string>()))
               .ReturnsAsync(RepositoryExistingRecordId)
               .Verifiable())
          .WhenICallGetTinyUrlAsync(longUri))
          .ThenTheExpectedResultShouldBe(expectedTinyUri)
          .ThenTheRepositoryShouldBeVerified();
    }

    [Fact]
    public async Task UrlService_GetTinyUrlForNewUrl_ReturnsExpected()
    {
        var longUri = new Uri("http://longUrl");
        var expectedTinyUri = new UriBuilder(UrlServiceSteps.TinyUrlBaseAddress)
        { Path = RepositoryExistingRecordIdEncodedValue }.Uri.AbsoluteUri;

        (await _steps
          .GivenIHaveUrlService()
          .GivenISetupRepository(x =>
              x.Setup(s => s.GetByPathAsync(It.IsAny<string>()))
               .ReturnsAsync(0)
               .Verifiable())
          .GivenISetupRepository(x =>
              x.Setup(s => s.CreateAsync(It.IsAny<string>()))
               .ReturnsAsync(RepositoryExistingRecordId)
               .Verifiable())
          .WhenICallGetTinyUrlAsync(longUri))
          .ThenTheExpectedResultShouldBe(expectedTinyUri)
          .ThenTheRepositoryShouldBeVerified();
    }
}