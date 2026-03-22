using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TinyUrl.Api.Core;
using TinyUrl.Api.Helpers;

namespace TinyUrl.Api.Handlers;

public sealed class TinyUrlHandler : IHttpRequestHandler
{
    private readonly IUrlService _urlService;
    private readonly ILogger<TinyUrlHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TinyUrlHandler"/> class.
    /// </summary>
    public TinyUrlHandler(IUrlService urlService, ILogger<TinyUrlHandler> logger)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IActionResult> HandleAsync(HttpRequest req)
    {
        const string keyName = "Url";
        if (!req.Headers.TryGetValue(keyName, out StringValues urlValue) &&
            !req.Query.TryGetValue(keyName, out urlValue))
        {
            _logger.LogError("Error. No Url provided.");
            return new BadRequestObjectResult("Error. Please provide the Url either in header or query param as \"Url\"");
        }

        var urlString = urlValue.FirstOrDefault();
        if (!TryValidateUrl(urlString, out Uri? uri))
        {
            _logger.LogError("Error. Invalid URL supplied: {Url}", urlString);
            return new BadRequestObjectResult("Error. Please provide a valid Url");
        }

        if (_urlService.IsTinyUrl(uri!))
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(uri!);
            return string.IsNullOrWhiteSpace(originalUrl)
                ? new BadRequestObjectResult("Error: Invalid tiny Url")
                : new OkObjectResult(originalUrl);
        }

        var tinyUrl = await _urlService.GetTinyUrlAsync(uri!);
        return new OkObjectResult(tinyUrl);
    }

    private static bool TryValidateUrl(string? urlString, out Uri? uri)
    {
        if (string.IsNullOrWhiteSpace(urlString) ||
            !Uri.IsWellFormedUriString(urlString, UriKind.Absolute) ||
            !Uri.TryCreate(urlString, UriKind.Absolute, out uri))
        {
            uri = null;
            return false;
        }

        return true;
    }
}
