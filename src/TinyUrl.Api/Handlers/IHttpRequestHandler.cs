using Microsoft.AspNetCore.Mvc;

namespace TinyUrl.Api.Handlers;

public interface IHttpRequestHandler
{
    /// <summary>
    /// Handle the HTTP request.
    /// </summary>
    Task<IActionResult> HandleAsync(HttpRequest req);
}
