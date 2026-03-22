namespace TinyUrl.Api.Core;

public interface IRepository
{
    /// <summary>
    /// Search by id and get original url string.
    /// </summary>
    /// <param name="id">The id to search for.</param>
    /// <returns>Returns the original url string, or null if not found.</returns>
    Task<string?> GetByIdAsync(int id);

    /// <summary>
    /// Search by original url string and get id.
    /// </summary>
    /// <param name="originalUrl">The original url string to search.</param>
    /// <returns>Returns the Id, or 0 if not found.</returns>
    Task<int> GetByPathAsync(string originalUrl);

    /// <summary>
    /// Persist a new URL and return the generated id.
    /// </summary>
    /// <param name="originalUrl">The long URL to store.</param>
    /// <returns>The newly created id.</returns>
    Task<int> CreateAsync(string originalUrl);
}
