using System.Diagnostics.CodeAnalysis;
using Dapper;
using Npgsql;

namespace TinyUrl.Api.Core;

[ExcludeFromCodeCoverage]
public sealed class PostgresRepository : IRepository
{
    private readonly string _connectionString;

    public PostgresRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<string?> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT original_url FROM short_url WHERE id = @id",
            new { id });
    }

    /// <inheritdoc/>
    public async Task<int> GetByPathAsync(string originalUrl)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT id FROM short_url WHERE original_url = @originalUrl",
                new { originalUrl });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(string originalUrl)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            // PostgreSQL equivalent of T-SQL's OUTPUT Inserted.Id is RETURNING id
            return await connection.QuerySingleAsync<int>(
                "INSERT INTO short_url (original_url) VALUES (@originalUrl) RETURNING id",
                new { originalUrl });
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}