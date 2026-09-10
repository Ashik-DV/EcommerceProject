using System.Text;

using ECommerceBackend.Data;
using ECommerceBackend.Helpers;
using ECommerceBackend.Models;
using ECommerceBackend.Repositories;
using ECommerceBackend.Repositories.Interfaces;
using ECommerceBackend.Services;
using ECommerceBackend.Services.Interfaces;
using ECommerceBackend.Logging;
using ECommerceBackend.Middleware;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1. LOGGING
// ======================================================

// Keep ASP.NET Core console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ======================================================
// 2. CUSTOM APPLICATION LOGGING
// ======================================================

builder.Services.Configure<LoggingSettings>(
builder.Configuration.GetSection("LoggingSettings"));

var loggingProvider =
builder.Configuration
.GetSection("LoggingSettings")
.GetValue<string>("Provider");

Console.WriteLine("================================");
Console.WriteLine("LOGGING CONFIGURATION");
Console.WriteLine("================================");
Console.WriteLine(
$"Environment: {builder.Environment.EnvironmentName}");

Console.WriteLine(
$"Content Root: {builder.Environment.ContentRootPath}");

Console.WriteLine(
$"Logging Provider: {loggingProvider ?? "(not configured)"}");

Console.WriteLine("================================");

if (string.Equals(
loggingProvider,
"Database",
StringComparison.OrdinalIgnoreCase))
{
builder.Services.AddScoped<
IApplicationLogger,
DatabaseLogger>();
}
else
{
builder.Services.AddSingleton<
IApplicationLogger,
FileLogger>();
}

// ======================================================
// 3. CONTROLLERS
// ======================================================

builder.Services.AddControllers();

// ======================================================
// 4. CORS
// React Frontend: http://localhost:5173
// ======================================================

builder.Services.AddCors(options =>
{
options.AddPolicy(
"ReactPolicy",
policy =>
{
policy
.WithOrigins(
"http://localhost:5173")
.AllowAnyHeader()
.AllowAnyMethod();
});
});

// ======================================================
// 5. MYSQL + ENTITY FRAMEWORK CORE
// ======================================================

var connectionString =
builder.Configuration.GetConnectionString(
"DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
throw new InvalidOperationException(
"DefaultConnection is missing. Check appsettings.json.");
}

builder.Services.AddDbContext<AppDbContext>(
options =>
{
options.UseMySql(
connectionString,
ServerVersion.AutoDetect(
connectionString));
});

// ======================================================
// 6. JWT CONFIGURATION
// ======================================================

var jwtSection =
builder.Configuration.GetSection("Jwt");

var jwtKey =
jwtSection["Key"];

var jwtIssuer =
jwtSection["Issuer"];

var jwtAudience =
jwtSection["Audience"];

Console.WriteLine("================================");
Console.WriteLine("JWT CONFIGURATION");
Console.WriteLine("================================");

Console.WriteLine(
$"JWT Key exists: {!string.IsNullOrWhiteSpace(jwtKey)}");

Console.WriteLine(
$"JWT Issuer: {jwtIssuer}");

Console.WriteLine(
$"JWT Audience: {jwtAudience}");

Console.WriteLine("================================");

// ======================================================
// 7. VALIDATE JWT CONFIGURATION
// ======================================================

if (string.IsNullOrWhiteSpace(jwtKey))
{
throw new InvalidOperationException(
"JWT Key is missing. Check appsettings.json and appsettings.Development.json.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
throw new InvalidOperationException(
"JWT Issuer is missing. Check appsettings.json and appsettings.Development.json.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
throw new InvalidOperationException(
"JWT Audience is missing. Check appsettings.json and appsettings.Development.json.");
}

// ======================================================
// 8. JWT AUTHENTICATION
// ======================================================

builder.Services
.AddAuthentication(
JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
options =>
{
options.TokenValidationParameters =
new TokenValidationParameters
{
ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtIssuer,

                ValidAudience =
                    jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtKey))
            };
    });

// ======================================================
// 9. AUTHORIZATION
// ======================================================

builder.Services.AddAuthorization();

// ======================================================
// 10. DEPENDENCY INJECTION
// ======================================================

// ------------------------------------------------------
// User Repository
// ------------------------------------------------------

builder.Services.AddScoped<
IUserRepository,
UserRepository>();

// ------------------------------------------------------
// Authentication Service
// ------------------------------------------------------

builder.Services.AddScoped<
IAuthService,
AuthService>();

// ------------------------------------------------------
// Product Repository
// ------------------------------------------------------

builder.Services.AddScoped<
IProductRepository,
ProductRepository>();

// ------------------------------------------------------
// Product Service
// ------------------------------------------------------

builder.Services.AddScoped<
IProductService,
ProductService>();

// ------------------------------------------------------
// Cart Repository
// ------------------------------------------------------

builder.Services.AddScoped<
ICartRepository,
CartRepository>();

// ------------------------------------------------------
// Cart Service
// ------------------------------------------------------

builder.Services.AddScoped<
ICartService,
CartService>();

// ------------------------------------------------------
// Order Repository
// ------------------------------------------------------

builder.Services.AddScoped<
IOrderRepository,
OrderRepository>();

// ------------------------------------------------------
// Order Service
// ------------------------------------------------------

builder.Services.AddScoped<
IOrderService,
OrderService>();

// ------------------------------------------------------
// Fake Payment Service
// ------------------------------------------------------

builder.Services.AddScoped<
IFakePaymentService,
FakePaymentService>();

// ------------------------------------------------------
// Password Helper
// ------------------------------------------------------

builder.Services.AddScoped<
PasswordHelper>();

// ------------------------------------------------------
// JWT Helper
// ------------------------------------------------------

builder.Services.AddScoped<
JwtHelper>();

// ======================================================
// 11. SWAGGER
// ======================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
options =>
{
options.AddSecurityDefinition(
"Bearer",
new OpenApiSecurityScheme
{
Name =
"Authorization",

            Type =
                SecuritySchemeType.Http,

            Scheme =
                "bearer",

            BearerFormat =
                "JWT",

            In =
                ParameterLocation.Header,

            Description =
                "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
            });
});

// ======================================================
// 12. BUILD APPLICATION
// ======================================================

var app =
builder.Build();

// ======================================================
// 13. DATABASE MIGRATION + ADMIN CREATION
// ======================================================

try
{
using var scope =
app.Services.CreateScope();

var dbContext =
    scope.ServiceProvider
        .GetRequiredService<
            AppDbContext>();

Console.WriteLine(
    "Checking database...");

// --------------------------------------------------
// Apply pending migrations
// --------------------------------------------------

await dbContext.Database.MigrateAsync();

Console.WriteLine(
    "Database migration completed.");

// --------------------------------------------------
// Admin credentials
// --------------------------------------------------

const string adminEmail =
    "admin@ecommerce.com";

const string adminPassword =
    "Admin@123";

// --------------------------------------------------
// Check Admin
// --------------------------------------------------

var adminExists =
    await dbContext.Users.AnyAsync(
        u =>
            u.Email ==
            adminEmail);

// --------------------------------------------------
// Create Admin
// --------------------------------------------------

if (!adminExists)
{
    var passwordHelper =
        scope.ServiceProvider
            .GetRequiredService<
                PasswordHelper>();

    var admin =
        new User
        {
            Name =
                "Admin",

            Email =
                adminEmail,

            PasswordHash =
                passwordHelper.HashPassword(
                    adminPassword),

            Role =
                "Admin"
        };

    dbContext.Users.Add(admin);

    await dbContext.SaveChangesAsync();

    Console.WriteLine(
        "Default admin user created.");

    Console.WriteLine(
        $"Admin Email: {adminEmail}");

    Console.WriteLine(
        $"Admin Password: {adminPassword}");
}
else
{
    Console.WriteLine(
        "Admin user already exists.");
}

}
catch (Exception ex)
{
Console.WriteLine("================================");
Console.WriteLine("DATABASE STARTUP ERROR");
Console.WriteLine("================================");
Console.WriteLine(ex.Message);
Console.WriteLine("================================");

throw;

}

// ======================================================
// 14. SWAGGER
// ======================================================

if (app.Environment.IsDevelopment())
{
app.UseSwagger();

app.UseSwaggerUI();

}

// ======================================================
// 15. HTTPS
// ======================================================

app.UseHttpsRedirection();

// ======================================================
// 16. CORS
// ======================================================

app.UseCors(
"ReactPolicy");

// ======================================================
// 17. CUSTOM REQUEST LOGGING
// ======================================================
//
// HTTP Request
// ↓
// CustomRequestLoggingMiddleware
// ↓
// IApplicationLogger
// ↓
// DatabaseLogger
// ↓
// ApplicationLogs
//
// ======================================================

app.UseMiddleware<
CustomRequestLoggingMiddleware>();

// ======================================================
// 18. AUTHENTICATION
// ======================================================

app.UseAuthentication();

// ======================================================
// 19. AUTHORIZATION
// ======================================================

app.UseAuthorization();

// ======================================================
// 20. CONTROLLERS
// ======================================================

app.MapControllers();

// ======================================================
// 21. RUN
// ======================================================

app.Run();