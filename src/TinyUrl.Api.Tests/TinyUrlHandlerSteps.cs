using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TinyUrl.Api.Core;
using TinyUrl.Api.Handlers;

namespace TinyUrl.Api.Tests;

internal sealed class TinyUrlHandlerSteps
{
    private readonly Mock<IUrlService> _urlService = new();
    private readonly Mock<ILogger<TinyUrlHandler>> _logger = new();

    private TinyUrlHandler _handler = null!;
    private IActionResult _handlerResult = null!;

    public TinyUrlHandlerSteps GivenIHaveHandler()
    {
        _handler = new TinyUrlHandler(_urlService.Object, _logger.Object);
        return this;
    }

    public TinyUrlHandlerSteps GivenISetupUrlService(Action<Mock<IUrlService>> setup)
    {
        setup(_urlService);
        return this;
    }

    public async Task<TinyUrlHandlerSteps> WhenIHandleTheRequestAsync(HttpRequest request)
    {
        _handlerResult = await _handler.HandleAsync(request);
        return this;
    }

    public async Task<TinyUrlHandlerSteps> WhenIHandleTheRequestAsync(string reqContent, bool isContentInHeader = true)
    {
        var request = CreateRequest(reqContent, isContentInHeader);
        _handlerResult = await _handler.HandleAsync(request);
        return this;
    }

    public TinyUrlHandlerSteps ThenTheResponseShouldBeOk()
    {
        _handlerResult.Should().BeOfType<OkObjectResult>();
        return this;
    }

    public TinyUrlHandlerSteps ThenTheUrlServiceShouldBeVerified()
    {
        _urlService.Verify();
        return this;
    }

    public TinyUrlHandlerSteps ThenTheResponseShouldBeBadRequest()
    {
        var result = _handlerResult.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.Value.Should().BeOfType<string>();
        return this;
    }

    private static HttpRequest CreateRequest(string content, bool isContentInHeader = true)
    {
        var httpContext = new DefaultHttpContext();
        if (isContentInHeader)
            httpContext.Request.Headers["Url"] = content;
        else
            httpContext.Request.QueryString = new QueryString($"?Url={Uri.EscapeDataString(content)}");

        return httpContext.Request;
    }
}
