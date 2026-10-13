using MySqlConnector;

namespace FragranceInfo.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync()
    {
        await CreateDatabaseAsync();
        await CreateTableAsync();
        await AddSampleDataAsync();
    }

    private static async Task CreateDatabaseAsync()
    {
        var settings = new MySqlConnectionStringBuilder(DatabaseConnection.Instance.ConnectionString);
        var databaseName = settings.Database;
        settings.Database = string.Empty;

        await using var connection = new MySqlConnection(settings.ConnectionString);
        await connection.OpenAsync();

        var quotedName = "`" + databaseName.Replace("`", "``") + "`";
        var sql = $"CREATE DATABASE IF NOT EXISTS {quotedName} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task CreateTableAsync()
    {
        const string sql = @"
            CREATE TABLE IF NOT EXISTS fragrances (
                id            INT          NOT NULL AUTO_INCREMENT,
                name          VARCHAR(100) NOT NULL,
                brand         VARCHAR(100) NOT NULL,
                release_year  INT          NULL,
                gender        VARCHAR(20)  NOT NULL,
                concentration VARCHAR(30)  NOT NULL,
                notes         VARCHAR(500) NULL,
                PRIMARY KEY (id)
            );";

        await using var connection = await DatabaseConnection.Instance.OpenConnectionAsync();
        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task AddSampleDataAsync()
    {
        await using var connection = await DatabaseConnection.Instance.OpenConnectionAsync();

        await using var countCommand = new MySqlCommand("SELECT COUNT(*) FROM fragrances;", connection);
        var count = Convert.ToInt64(await countCommand.ExecuteScalarAsync());
        if (count > 0)
        {
            return;
        }

        const string sql = @"
            INSERT INTO fragrances (name, brand, release_year, gender, concentration, notes) VALUES
            ('Sauvage', 'Dior', 2015, 'Male', 'Eau de Toilette', 'Bergamot, pepper, ambroxan'),
            ('Bleu de Chanel', 'Chanel', 2010, 'Male', 'Eau de Toilette', 'Grapefruit, incense, ginger, cedar'),
            ('Coco Mademoiselle', 'Chanel', 2001, 'Female', 'Eau de Parfum', 'Orange, rose, jasmine, patchouli'),
            ('Black Opium', 'Yves Saint Laurent', 2014, 'Female', 'Eau de Parfum', 'Coffee, vanilla, white flowers'),
            ('Tobacco Vanille', 'Tom Ford', 2007, 'Unisex', 'Eau de Parfum', 'Tobacco leaf, vanilla, tonka bean, spices'),
            ('Aventus', 'Creed', 2010, 'Male', 'Eau de Parfum', 'Pineapple, birch, musk, oakmoss');";

        await using var insertCommand = new MySqlCommand(sql, connection);
        await insertCommand.ExecuteNonQueryAsync();
    }
}
