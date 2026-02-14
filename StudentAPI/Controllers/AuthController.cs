using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);

            // For demo purposes, using hardcoded credentials
            // In production, validate against database with hashed passwords
            if (IsValidUser(request.Email, request.Password))
            {
                var token = GenerateJwtToken(request.Email);
                
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        token = token,
                        user = new
                        {
                            email = request.Email,
                            name = GetUserName(request.Email),
                            role = GetUserRole(request.Email)
                        }
                    },
                    message = "Login successful"
                });
            }

            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return Unauthorized(new
            {
                success = false,
                message = "Invalid email or password"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Email}", request.Email);
            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error during login"
            });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registration attempt for user: {Email}", request.Email);

            // For demo purposes, accepting any registration
            // In production, validate email format, password strength, and store in database
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email and password are required"
                });
            }

            // Simulate user creation success
            var token = GenerateJwtToken(request.Email);

            return Ok(new
            {
                success = true,
                data = new
                {
                    token = token,
                    user = new
                    {
                        email = request.Email,
                        name = request.Name,
                        role = "User"
                    }
                },
                message = "Registration successful"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Email}", request.Email);
            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error during registration"
            });
        }
    }

    private bool IsValidUser(string email, string password)
    {
        // Demo credentials - in production, validate against database
        var demoUsers = new Dictionary<string, string>
        {
            { "admin@school.com", "admin123" },
            { "teacher@school.com", "teacher123" },
            { "demo@example.com", "demo123" },
            { "test@test.com", "test123" }
        };

        return demoUsers.ContainsKey(email.ToLower()) && 
               demoUsers[email.ToLower()] == password;
    }

    private string GetUserName(string email)
    {
        var userNames = new Dictionary<string, string>
        {
            { "admin@school.com", "School Administrator" },
            { "teacher@school.com", "Teacher User" },
            { "demo@example.com", "Demo User" },
            { "test@test.com", "Test User" }
        };

        return userNames.GetValueOrDefault(email.ToLower(), email.Split('@')[0]);
    }

    private string GetUserRole(string email)
    {
        var userRoles = new Dictionary<string, string>
        {
            { "admin@school.com", "Admin" },
            { "teacher@school.com", "Teacher" },
            { "demo@example.com", "User" },
            { "test@test.com", "User" }
        };

        return userRoles.GetValueOrDefault(email.ToLower(), "User");
    }

    private string GenerateJwtToken(string email)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters-long";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "StudentManagementAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "StudentManagementApp";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", GetUserRole(email))
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}