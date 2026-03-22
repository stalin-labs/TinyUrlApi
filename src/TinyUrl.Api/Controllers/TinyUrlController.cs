using Microsoft.AspNetCore.Mvc;
using TinyUrl.Api.Handlers;

namespace TinyUrl.Api.Controllers;

/// <summary>
/// Controller for TinyUrl services.
/// Preserves the original route: GET /v1/TinyUrl/
/// </summary>
[ApiController]
[Route("v1/[controller]")]
public sealed class TinyUrlController : ControllerBase
{
    private readonly IHttpRequestHandler _handler;
    private readonly ILogger<TinyUrlController> _logger;

    public TinyUrlController(IHttpRequestHandler handler, ILogger<TinyUrlController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    /// <summary>
    /// Create or resolve a tiny URL.
    /// Pass the URL to shorten/expand via the "Url" header or query parameter.
    /// </summary>
    /// <remarks>
    /// - If the supplied URL is already a tiny URL, returns the original long URL.
    /// - If the supplied URL is a long URL, returns a shortened tiny URL.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get()
    {
        try
        {
            return await _handler.HandleAsync(Request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error processing TinyUrl request.");
            return BadRequest($"ERROR: {ex.Message}");
        }
    }
}
// https://localhost:52349/v1/TinyUrl?Url=http://mywebsite.com/Users/Display?id=123#email