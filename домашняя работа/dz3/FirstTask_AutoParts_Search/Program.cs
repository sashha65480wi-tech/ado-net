using FirstTask_AutoParts_Search.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DbHelper>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<DbHelper>().Initialize();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Parts}/{action=Index}/{id?}");
app.Run();
