using MySqlConnector;

namespace FragranceInfo.Data;

// Singleton design pattern.
// The constructor is private, so the class can't be created with "new" from outside.
// There is only one instance, stored in a static field and created on first use
// (Lazy<T> makes this thread safe). Everything gets it through DatabaseConnection.Instance.
public sealed class DatabaseConnection
{
    private static readonly Lazy<DatabaseConnection> _instance =
        new(() => new DatabaseConnection());

    private string? _connectionString;

    private DatabaseConnection()
    {
    }

    public static DatabaseConnection Instance => _instance.Value;

    public string ConnectionString =>
        _connectionString ?? throw new InvalidOperationException(
            "DatabaseConnection is not configured. Call Configure() in Program.cs first.");

    public void Configure(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("The connection string is empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<MySqlConnection> OpenConnectionAsync()
    {
        var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}
