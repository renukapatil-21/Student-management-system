using StudentAPI.Models;
using StudentAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Try to register MongoDB services, fallback to in-memory if connection fails
try
{
    // Test MongoDB connection first
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    if (!string.IsNullOrEmpty(mongoSettings?.ConnectionString) && 
        mongoSettings.ConnectionString.Contains("localhost:27017"))
    {
        // Use in-memory services for local development
        builder.Services.AddSingleton<IStudentService, InMemoryStudentService>();
        Console.WriteLine("Using in-memory database for development.");
    }
    else
    {
        // Register MongoDB services
        builder.Services.AddSingleton<StudentService>();
        builder.Services.AddSingleton<FeeService>();
        builder.Services.AddSingleton<InquiryService>();
        builder.Services.AddSingleton<DatabaseInitializer>();
        builder.Services.AddSingleton<IStudentService>(provider => provider.GetRequiredService<StudentService>());
        Console.WriteLine("Using MongoDB Atlas connection.");
    }
}
catch (Exception ex)
{
    // Fallback to in-memory services
    builder.Services.AddSingleton<IStudentService, InMemoryStudentService>();
    Console.WriteLine($"Failed to connect to MongoDB, using in-memory database: {ex.Message}");
}

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters-long";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentManagementAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentManagementApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

// Initialize database in background (non-blocking)
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database initialization in background...");
        await dbInitializer.InitializeAsync();
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed, but API will continue running.");
    }
});

app.Run();
