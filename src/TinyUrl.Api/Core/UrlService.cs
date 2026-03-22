using TinyUrl.Api.Helpers;

namespace TinyUrl.Api.Core;

public class UrlService : IUrlService
{
    private readonly IRepository _repository;
    private readonly Uri _tinyUrlBaseAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlService"/> class.
    /// </summary>
    public UrlService(IRepository repository, Uri tinyUrlBaseAddress)
    {
        _repository = repository;
        _tinyUrlBaseAddress = tinyUrlBaseAddress;
    }

    /// <inheritdoc/>
    public bool IsTinyUrl(Uri uri)
    {
        return string.Equals(uri.Host, _tinyUrlBaseAddress.Host, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public async Task<string> GetOriginalUrlAsync(Uri tinyUri)
    {
        if (ShortUrlHelpers.TryDecode(tinyUri.LocalPath?.Substring(1), out int id))
        {
            return await _repository.GetByIdAsync(id) ?? string.Empty;
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public async Task<string> GetTinyUrlAsync(Uri longUri)
    {
        // Check if the URL already exists to avoid duplicates
        int id = await _repository.GetByPathAsync(longUri.OriginalString);
        if (id == 0)
        {
            id = await _repository.CreateAsync(longUri.OriginalString);
        }

        var uriBuilder = new UriBuilder(_tinyUrlBaseAddress)
        {
            Path = ShortUrlHelpers.Encode(id)
        };

        return uriBuilder.Uri.AbsoluteUri;
    }
}