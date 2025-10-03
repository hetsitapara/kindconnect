using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1) Read connection string from appsettings.json (DefaultConnection)
// 2) Register EF Core DbContext for SQL Server
// 3) Configure ASP.NET Core Identity for authentication & roles
// 4) Add MVC controllers with views and set global authorization policy (auth required by default)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity configuration (password rules, no email confirmation required for demo)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
// Require authenticated users by default
// Any action that should allow anonymous access must explicitly use [AllowAnonymous]
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // For development you could enable migration endpoints. Disabled here.
    // app.UseMigrationsEndPoint();
}
else
{
    // In production: show user-friendly error page and enable HSTS (HTTP Strict Transport Security)
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP -> HTTPS and serve static files from wwwroot
app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable endpoint routing
app.UseRouting();

// Enable authentication/authorization middlewares (order matters)
app.UseAuthentication();
app.UseAuthorization();

// Conventional route mapping: defaults to Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the database
// This runs once on application startup. It ensures roles/users exist and inserts sample data.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        await SeedData.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();
