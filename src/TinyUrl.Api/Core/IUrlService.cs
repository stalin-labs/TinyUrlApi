namespace TinyUrl.Api.Core;

public interface IUrlService
{
    /// <summary>
    /// Check whether the given URL is already a tiny URL.
    /// </summary>
    bool IsTinyUrl(Uri uri);

    /// <summary>
    /// Get the original long URL from a tiny URL.
    /// </summary>
    Task<string> GetOriginalUrlAsync(Uri tinyUri);

    /// <summary>
    /// Get (or create) a tiny URL for the given long URL.
    /// </summary>
    Task<string> GetTinyUrlAsync(Uri longUri);
}
