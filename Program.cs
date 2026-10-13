using FragranceInfo.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "The connection string 'DefaultConnection' is missing in appsettings.json.");
DatabaseConnection.Instance.Configure(connectionString);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FragranceRepository>();

var app = builder.Build();

try
{
    await DatabaseInitializer.InitializeAsync();
}
catch (MySqlException exception)
{
    app.Logger.LogCritical(exception,
        "Cannot connect to MySQL. Start MySQL in the XAMPP Control Panel and check " +
        "the connection string in appsettings.json, then run the application again.");
    Environment.ExitCode = 1;
    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Fragrances}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
