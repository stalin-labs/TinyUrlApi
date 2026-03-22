using Microsoft.AspNetCore.Http;
using Moq;
using TinyUrl.Api.Core;
using TinyUrl.Api.Handlers;
using Xunit;

namespace TinyUrl.Api.Tests;

public sealed class TinyUrlHandlerTests
{
    private readonly TinyUrlHandlerSteps _steps = new();

    private const string LongUrl =
        "https://www.amazon.in/s?i=electronics&bbn=1389402031&rh=n%3A976419031%2Cn%3A976420031%2Cn%3A1389401031%2Cn%3A1389402031%2Cp_89%3AMivi%2Cp_85%3A10440599031&pf_rd_i=1389402031&pf_rd_m=A1K21FY43GMZF8&pf_rd_p=d04c3d0d-51c2-40a5-a944-a74c24fb7927&pf_rd_r=C94CN6HV32QC0A4QQTDM&pf_rd_s=merchandised-search-3&pf_rd_t=101&ref=s9_acss_bw_cg_AugART_9a1_w";

    [Fact]
    public void Handler_InitializeWithNullUrlService_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TinyUrlHandler(null!, Mock.Of<Microsoft.Extensions.Logging.ILogger<TinyUrlHandler>>()));
    }

    [Fact]
    public void Handler_InitializeWithNullLogger_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TinyUrlHandler(Mock.Of<IUrlService>(), null!));
    }

    [Fact]
    public async Task Handler_HandleRequestWithNoUrl_ReturnsBadRequest()
    {
        var emptyReq = new DefaultHttpContext().Request;

        (await _steps
            .GivenIHaveHandler()
            .WhenIHandleTheRequestAsync(emptyReq))
            .ThenTheResponseShouldBeBadRequest();
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData(" ", false)]
    [InlineData("http://", true)]
    [InlineData("h://localhost", true)]
    public async Task Handler_HandleInvalidRequestData_ReturnsBadRequest(string reqContent, bool isContentInHeader)
    {
        (await _steps
            .GivenIHaveHandler()
            .WhenIHandleTheRequestAsync(reqContent, isContentInHeader))
            .ThenTheResponseShouldBeBadRequest();
    }

    [Theory]
    [InlineData("https://MyTinyUrl")]
    public async Task Handler_HandleTinyUrl_ReturnsOk(string tinyUrl)
    {
        (await _steps
            .GivenIHaveHandler()
            .GivenISetupUrlService(x => x
                .Setup(s => s.IsTinyUrl(It.IsAny<Uri>()))
                .Returns(true)
                .Verifiable())
            .GivenISetupUrlService(x => x
                .Setup(s => s.GetOriginalUrlAsync(It.IsAny<Uri>()))
                .ReturnsAsync("https://OriginalLongUrl")
                .Verifiable())
            .WhenIHandleTheRequestAsync(tinyUrl))
            .ThenTheResponseShouldBeOk()
            .ThenTheUrlServiceShouldBeVerified();
    }

    [Theory]
    [InlineData("https://InvalidTinyUrl")]
    public async Task Handler_HandleInvalidTinyUrl_ReturnsBadRequest(string invalidTinyUrl)
    {
        (await _steps
            .GivenIHaveHandler()
            .GivenISetupUrlService(x => x
                .Setup(s => s.IsTinyUrl(It.IsAny<Uri>()))
                .Returns(true)
                .Verifiable())
            .GivenISetupUrlService(x => x
                .Setup(s => s.GetOriginalUrlAsync(It.IsAny<Uri>()))
                .ReturnsAsync(string.Empty)
                .Verifiable())
            .WhenIHandleTheRequestAsync(invalidTinyUrl))
            .ThenTheResponseShouldBeBadRequest();
    }

    [Theory]
    [InlineData("http://localhost")]
    [InlineData("http://mywebsite.com")]
    [InlineData("https://mywebsite.com")]
    [InlineData("ftp://myftp.com")]
    [InlineData("sftp://host.myftp.com:22/")]
    [InlineData("http://mywebsite.com/Users")]
    [InlineData("http://mywebsite.com/Users/Display")]
    [InlineData("http://mywebsite.com/Users/Display?id=123")]
    [InlineData("http://mywebsite.com/Users/Display?id=123#email")]
    [InlineData(LongUrl)]
    public async Task Handler_HandleLongUrl_ReturnsOk(string longUrl)
    {
        (await _steps
            .GivenIHaveHandler()
            .GivenISetupUrlService(x => x
                .Setup(s => s.IsTinyUrl(It.IsAny<Uri>()))
                .Returns(false)
                .Verifiable())
            .GivenISetupUrlService(x => x
                .Setup(s => s.GetTinyUrlAsync(It.IsAny<Uri>()))
                .ReturnsAsync("https://MyTinyUrl")
                .Verifiable())
            .WhenIHandleTheRequestAsync(longUrl))
            .ThenTheResponseShouldBeOk()
            .ThenTheUrlServiceShouldBeVerified();
    }
}