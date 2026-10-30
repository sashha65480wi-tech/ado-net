using FirstTask_AutoParts_Search.Models;
using Npgsql;
namespace FirstTask_AutoParts_Search.Data;
public class DbHelper
{
    private readonly string _connectionString;
    public DbHelper(IConfiguration configuration) =>
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Не задана строка подключения DefaultConnection.");

    public void Initialize()
    {
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        const string sql = @"
            CREATE TABLE IF NOT EXISTS parts (
                id SERIAL PRIMARY KEY, name TEXT NOT NULL, article TEXT NOT NULL,
                price NUMERIC(12,2) NOT NULL, stock INTEGER NOT NULL DEFAULT 0
            );
            INSERT INTO parts(name, article, price, stock)
            SELECT 'Масляный фильтр', 'OF-1001', 450, 25
            WHERE NOT EXISTS (SELECT 1 FROM parts);
            INSERT INTO parts(name, article, price, stock)
            SELECT 'Воздушный фильтр', 'AF-2002', 620, 18
            WHERE NOT EXISTS (SELECT 1 FROM parts WHERE article='AF-2002');
            INSERT INTO parts(name, article, price, stock)
            SELECT 'Тормозные колодки', 'BP-3003', 2100, 12
            WHERE NOT EXISTS (SELECT 1 FROM parts WHERE article='BP-3003');";
        using var cmd = new NpgsqlCommand(sql, con);
        cmd.ExecuteNonQuery();
    }

    public List<Part> SearchParts(string? query)
    {
        var result = new List<Part>();
        using var con = new NpgsqlConnection(_connectionString);
        con.Open();
        var sql = string.IsNullOrWhiteSpace(query)
            ? "SELECT id, name, article, price, stock FROM parts ORDER BY id"
            : "SELECT id, name, article, price, stock FROM parts WHERE name ILIKE @q OR article ILIKE @q ORDER BY id";
        using var cmd = new NpgsqlCommand(sql, con);
        if (!string.IsNullOrWhiteSpace(query))
            cmd.Parameters.AddWithValue("q", $"%{query.Trim()}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(new Part {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Article = reader.GetString(reader.GetOrdinal("article")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Stock = reader.GetInt32(reader.GetOrdinal("stock"))
            });
        return result;
    }
}