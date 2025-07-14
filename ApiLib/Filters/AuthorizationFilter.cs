using CommonLib.Configuration;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace ApiLib.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AuthorizationFilter(IJwtService jwtService, IOptions<JwtSettings> jwtSettings)
        {
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // ✅ Check if AllowAnonymous is applied on Controller or Action
            var endpoint = context.HttpContext.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

            if (allowAnonymous)
            {
                Log.Information("Skipping Authorization for endpoint: {Endpoint}", endpoint?.DisplayName);
                return; // Skip JWT validation
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            Log.Information("Authorization Header received: {AuthHeader}", authHeader);

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning("Invalid or missing Authorization header: {AuthHeader}", authHeader);
                context.Result = new UnauthorizedObjectResult(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title = "Unauthorized",
                    status = 401,
                    traceId = context.HttpContext.TraceIdentifier
                });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            Log.Information("JWT Token extracted: {Token}", token);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key);

                var principal = await Task.Run(() => tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken)).ConfigureAwait(false);

                if (principal?.Identity == null)
                {
                    Log.Warning("JWT validation failed: No valid identity found in token: {Token}", token);
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                        title = "Unauthorized",
                        status = 401,
                        traceId = context.HttpContext.TraceIdentifier
                    });
                    return;
                }

                var username = principal.Identity.Name ?? string.Empty;
                Log.Information("JWT validated successfully for user: {Username}", username);
                context.HttpContext.Items["Username"] = username;
            }
            catch (SecurityTokenException ex)
            {
                Log.Error(ex, "Security token validation failed for token: {Token}. Exception: {Message}, StackTrace: {StackTrace}",
                    token, ex.Message, ex.StackTrace);
                context.Result = new UnauthorizedObjectResult(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title = "Unauthorized",
                    status = 401,
                    traceId = context.HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to validate JWT token: {Token}. Exception: {Message}, StackTrace: {StackTrace}",
                    token, ex.Message, ex.StackTrace);
                context.Result = new UnauthorizedObjectResult(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title = "Unauthorized",
                    status = 401,
                    traceId = context.HttpContext.TraceIdentifier
                });
            }
        }
    }
}