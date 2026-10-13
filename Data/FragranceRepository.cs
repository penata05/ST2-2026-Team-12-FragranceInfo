using System.Text;
using FragranceInfo.Models;
using MySqlConnector;

namespace FragranceInfo.Data;

public class FragranceRepository
{
    private readonly DatabaseConnection _database = DatabaseConnection.Instance;

    public async Task<int> InsertAsync(Fragrance fragrance)
    {
        const string sql = @"
            INSERT INTO fragrances (name, brand, release_year, gender, concentration, notes)
            VALUES (@name, @brand, @releaseYear, @gender, @concentration, @notes);";

        await using var connection = await _database.OpenConnectionAsync();
        await using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", fragrance.Name.Trim());
        command.Parameters.AddWithValue("@brand", fragrance.Brand.Trim());
        command.Parameters.AddWithValue("@releaseYear", (object?)fragrance.ReleaseYear ?? DBNull.Value);
        command.Parameters.AddWithValue("@gender", fragrance.Gender);
        command.Parameters.AddWithValue("@concentration", fragrance.Concentration);
        command.Parameters.AddWithValue("@notes",
            string.IsNullOrWhiteSpace(fragrance.Notes) ? DBNull.Value : fragrance.Notes.Trim());

        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }

    public async Task<List<Fragrance>> GetFilteredAsync(FragranceFilter filter)
    {
        var sql = new StringBuilder(@"
            SELECT id, name, brand, release_year, gender, concentration, notes
            FROM fragrances
            WHERE 1 = 1");

        await using var connection = await _database.OpenConnectionAsync();
        await using var command = new MySqlCommand { Connection = connection };

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            sql.Append(" AND name LIKE @name");
            command.Parameters.AddWithValue("@name", "%" + EscapeLike(filter.Name.Trim()) + "%");
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            sql.Append(" AND brand = @brand");
            command.Parameters.AddWithValue("@brand", filter.Brand.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filter.Gender))
        {
            sql.Append(" AND gender = @gender");
            command.Parameters.AddWithValue("@gender", filter.Gender.Trim());
        }

        sql.Append(" ORDER BY brand, name;");
        command.CommandText = sql.ToString();

        var fragrances = new List<Fragrance>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            fragrances.Add(new Fragrance
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Brand = reader.GetString("brand"),
                ReleaseYear = reader.IsDBNull(reader.GetOrdinal("release_year"))
                    ? null
                    : reader.GetInt32("release_year"),
                Gender = reader.GetString("gender"),
                Concentration = reader.GetString("concentration"),
                Notes = reader.IsDBNull(reader.GetOrdinal("notes"))
                    ? null
                    : reader.GetString("notes"),
            });
        }

        return fragrances;
    }

    public async Task<List<string>> GetBrandsAsync()
    {
        const string sql = "SELECT DISTINCT brand FROM fragrances ORDER BY brand;";

        await using var connection = await _database.OpenConnectionAsync();
        await using var command = new MySqlCommand(sql, connection);

        var brands = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            brands.Add(reader.GetString(0));
        }

        return brands;
    }

    private static string EscapeLike(string text) =>
        text.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
}
