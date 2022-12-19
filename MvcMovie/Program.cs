using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("TEST_CONNECTION");

// Replace with your server version and type.
// Use 'MariaDbServerVersion' for MariaDB.
// Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
// For common usages, see pull request #1233.
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

// Replace 'YourDbContext' with the name of your own DbContext derived class.
builder.Services.AddDbContext<MvcMovieContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        // The following three options help with debugging, but should
        // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);


builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MvcMovieContext>();

// builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>	{
// 		options.Password.RequireUppercase = true; // on production add more secured options
// 		options.Password.RequireDigit = true;               
// 		options.SignIn.RequireConfirmedEmail = true;
// 	}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// dotnet-aspnet-codegenerator identity --dbContext MvcMovieContext --files "Account.Login;Account.Logout;Account.Register"

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();