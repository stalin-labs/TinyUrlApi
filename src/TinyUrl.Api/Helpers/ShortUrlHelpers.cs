using System.Text;

namespace TinyUrl.Api.Helpers;

public static class ShortUrlHelpers
{
    /// <summary>
    /// Returns a Base64-encoded string for the given integer id.
    /// </summary>
    public static string Encode(int id)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(id.ToString());
        return Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Attempts to decode a Base64 string back to an integer id.
    /// </summary>
    /// <returns><see langword="true"/> if successfully decoded; <see langword="false"/> otherwise.</returns>
    public static bool TryDecode(string? encodedData, out int id)
    {
        try
        {
            var base64EncodedBytes = Convert.FromBase64String(encodedData ?? string.Empty);
            var decoded = Encoding.UTF8.GetString(base64EncodedBytes);
            return int.TryParse(decoded, out id);
        }
        catch
        {
            id = 0;
            return false;
        }
    }
}
