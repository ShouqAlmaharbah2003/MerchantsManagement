using CommonLib.Configuration;
using CommonLib.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;
using System;
using System.Threading.Tasks;

namespace CommonLib.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings), "JWT settings cannot be null.");
            if (string.IsNullOrEmpty(_jwtSettings.Key))
                throw new ArgumentException("JWT Key cannot be null or empty.", nameof(_jwtSettings.Key));
            if (Encoding.UTF8.GetBytes(_jwtSettings.Key).Length < 16)
                throw new ArgumentException("JWT Key must be at least 16 bytes long.", nameof(_jwtSettings.Key));

            Log.Information("JwtSettings loaded: Issuer={Issuer}, Audience={Audience}, ExpiryInMinutes={Expiry}",
                _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.ExpiryInMinutes);
        }

        public async Task<string> GenerateTokenAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                Log.Information("JWT generated successfully for user: {Username}, Token: {Token}", username, tokenString);
                return await Task.FromResult(tokenString).ConfigureAwait(false);
        }

        public async Task<(bool IsValid, string Username)> ValidateTokenAsync(string token)
        {
            string username = string.Empty; 
            if (string.IsNullOrEmpty(token))
            {
                Log.Warning("Token validation failed: Token is null or empty.");
                return await Task.FromResult((false, username)).ConfigureAwait(false);
            }

            try
            {
                Log.Information("Validating JWT: {Token}", token);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (principal?.Identity == null)
                {
                    Log.Warning("JWT validation failed: No valid identity found in token.");
                    return await Task.FromResult((false, username)).ConfigureAwait(false);
                }

                username = principal.Identity.Name ?? string.Empty;
                Log.Information("JWT validated successfully for user: {Username}", username);
                return await Task.FromResult((true, username)).ConfigureAwait(false);
            }
            catch (SecurityTokenException ex)
            {
                Log.Error(ex, "Security token validation failed for token: {Token}. Exception: {Message}, StackTrace: {StackTrace}",
                    token, ex.Message, ex.StackTrace);
                return await Task.FromResult((false, username)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to validate JWT token: {Token}. Exception: {Message}, StackTrace: {StackTrace}",
                    token, ex.Message, ex.StackTrace);
                return await Task.FromResult((false, username)).ConfigureAwait(false);
            }
        }
    }
}