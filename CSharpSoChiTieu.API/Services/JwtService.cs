using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CSharpSoChiTieu.Data;

namespace CSharpSoChiTieu.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(ct_User user);
        ClaimsPrincipal? ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(ct_User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["Jwt:Key"];

                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("JWT key is not configured");
                }

                var key = Encoding.UTF8.GetBytes(jwtKey);

                // HMAC-SHA256 yêu cầu ít nhất 256 bits (32 bytes)
                if (key.Length < 32)
                {
                    throw new InvalidOperationException($"JWT key must be at least 32 bytes (256 bits) long. Current key length: {key.Length} bytes");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "user"),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("FullName", user.FullName ?? "")
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(7), // Token expires in 7 days
                    Issuer = _configuration["Jwt:Issuer"] ?? "CSharpSoChiTieuAPI",
                    Audience = _configuration["Jwt:Audience"] ?? "CSharpSoChiTieuMobile",
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("JWT token generated successfully for user: {UserId}", user.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user: {UserId}", user?.Id);
                throw new InvalidOperationException("Failed to generate JWT token", ex);
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["Jwt:Key"];

                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT key is not configured for token validation");
                    return null;
                }

                var key = Encoding.UTF8.GetBytes(jwtKey);

                // HMAC-SHA256 yêu cầu ít nhất 256 bits (32 bytes)
                if (key.Length < 32)
                {
                    _logger.LogError($"JWT key is too short for validation. Required: 32 bytes, Current: {key.Length} bytes");
                    return null;
                }

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"] ?? "CSharpSoChiTieuAPI",
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"] ?? "CSharpSoChiTieuMobile",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                if (principal == null) return null;

                var userIdClaim = principal.FindFirst("UserId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                    return null;

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract user ID from token");
                return null;
            }
        }
    }
}