using ShopApp.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddSingleton(new DbHelper(connectionString));

var app = builder.Build();

// Дожидаемся PostgreSQL и создаём таблицу с начальными данными.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbHelper>();
    var retries = 10;
    var initialized = false;

    while (retries > 0 && !initialized)
    {
        try
        {
            db.Initialize();
            Console.WriteLine("База данных инициализирована.");
            initialized = true;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Ожидание PostgreSQL... Осталось попыток: {retries}. {ex.Message}");
            if (retries == 0) throw;
            Thread.Sleep(3000);
        }
    }
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
