namespace TinyUrl.Api.Options;

public sealed class TinyUrlOptions
{
    public const string SectionName = "TinyUrl";

    /// <summary>
    /// The base address used to detect and construct tiny URLs.
    /// e.g. "https://tiny.ul/"
    /// </summary>
    public string TinyUrlBaseAddress { get; set; } = string.Empty;
}
