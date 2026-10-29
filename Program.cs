using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
    ?? "Host=localhost;Port=5432;Database=appdb;Username=appuser;Password=supersecret";

// Проверяем подключение и создаём таблицу, если её ещё нет.
await using (var connection = new NpgsqlConnection(connectionString))
{
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand("""
        CREATE TABLE IF NOT EXISTS visits (
            id SERIAL PRIMARY KEY,
            path VARCHAR(255) NOT NULL,
            created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        );
        """, connection);
    await command.ExecuteNonQueryAsync();
}

app.MapGet("/", async () =>
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    int visitId;
    await using (var insert = new NpgsqlCommand(
        "INSERT INTO visits (path) VALUES (@path) RETURNING id;", connection))
    {
        insert.Parameters.AddWithValue("path", "/");
        visitId = Convert.ToInt32(await insert.ExecuteScalarAsync());
    }

    int totalVisits;
    await using (var count = new NpgsqlCommand("SELECT COUNT(*) FROM visits;", connection))
    {
        totalVisits = Convert.ToInt32(await count.ExecuteScalarAsync());
    }

    return Results.Ok(new
    {
        message = "Hello from ASP.NET Core + ADO.NET + PostgreSQL!",
        visit_id = visitId,
        total_visits = totalVisits
    });
});

app.MapGet("/visits", async () =>
{
    var visits = new List<Visit>();
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(
        "SELECT id, path, created_at FROM visits ORDER BY id DESC LIMIT 20;", connection);
    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        visits.Add(new Visit(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDateTime(2)));
    }

    return Results.Ok(visits);
});

app.MapGet("/health", async () =>
{
    try
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("SELECT 1;", connection);
        await command.ExecuteScalarAsync();
        return Results.Ok(new { status = "ok", db = "connected" });
    }
    catch
    {
        return Results.Json(new { status = "error", db = "disconnected" }, statusCode: 500);
    }
});

app.Run();

record Visit(int Id, string Path, DateTime CreatedAt);
