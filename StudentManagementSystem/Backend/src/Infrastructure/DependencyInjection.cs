using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StudentManagementSystem.Domain.Interfaces;
using StudentManagementSystem.Application.Interfaces.Repositories;
using StudentManagementSystem.Infrastructure.Persistence;
using StudentManagementSystem.Infrastructure.Persistence.Repositories;
using StudentManagementSystem.Infrastructure.Authentication;
using StudentManagementSystem.Application.Features.Users.Commands.LoginUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudentManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString("MongoDb") ?? 
                throw new InvalidOperationException("MongoDB connection string not found");
            return new MongoClient(connectionString);
        });

        services.AddScoped(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var databaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value ?? 
                throw new InvalidOperationException("MongoDB database name not found");
            return client.GetDatabase(databaseName);
        });

        // Repository Registration
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IFeeRepository, FeeRepository>();
        services.AddScoped<IInquiryRepository, InquiryRepository>();
        services.AddScoped<IDashboardAnalyticsRepository, DashboardAnalyticsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT Configuration
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? 
            throw new InvalidOperationException("JWT settings not found");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}

public class MongoDbSettings
{
    public string DatabaseName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}