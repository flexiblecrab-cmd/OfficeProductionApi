using OfficeProductionApi.DTOs;
using Microsoft.Data.Sqlite;

namespace OfficeProductionApi.Services;

public sealed class ReportService
{
    private readonly string _connectionString;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IConfiguration configuration,
        ILogger<ReportService> logger)
    {
        _connectionString =
           configuration.GetConnectionString("OfficeDatabase")
            ?? throw new InvalidOperationException(
                "The OfficeDatabase connection string is missing.");

        _logger = logger;
    }

    public async Task InitializeDatabaseAsync()
    {
        CreateDatabaseDirectoryIfNeeded();

        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Reports
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                CreatedBy TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL,
                IsActive INTEGER NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync();

        _logger.LogInformation(
            "Reports database table was verified successfully");
    }

    public async Task<int> GetActiveCountAsync()
    {
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT COUNT(*)
            FROM Reports
            WHERE IsActive = 1;
            """;

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<long> CreateReportAsync(
        CreateReportRequest request,
        string createdBy)
    {
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO Reports
            (
                Title,
                Description,
                CreatedBy,
                CreatedAtUtc,
                IsActive
            )
            VALUES
            (
                $title,
                $description,
                $createdBy,
                $createdAtUtc,
                1
            );

            SELECT last_insert_rowid();
            """;

        command.Parameters.AddWithValue(
            "$title",
            request.Title.Trim());

        command.Parameters.AddWithValue(
            "$description",
            request.Description.Trim());

        command.Parameters.AddWithValue(
            "$createdBy",
            createdBy);

        command.Parameters.AddWithValue(
            "$createdAtUtc",
            DateTime.UtcNow.ToString("O"));

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt64(result);
    }

    public async Task<bool> CanConnectAsync()
    {
        try
        {
            await using var connection =
                new SqliteConnection(_connectionString);

            await connection.OpenAsync();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT 1;";

            await command.ExecuteScalarAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void CreateDatabaseDirectoryIfNeeded()
    {
        var builder =
            new SqliteConnectionStringBuilder(_connectionString);

        string? databasePath = builder.DataSource;

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            return;
        }

        string fullPath = Path.GetFullPath(databasePath);
        string? directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}