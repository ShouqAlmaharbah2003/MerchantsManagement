using CommonLib.Dtos.Requests;
using CommonLib.Enums;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] 
    public class AuthController : SecureController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IUserService userService, ILogger<AuthController> logger,
            ILocalizationService localizationService)
            : base(localizationService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return Error(ErrorCodes.ValidationError); 
            }

            try
            {
                var user = await _userService.RegisterAsync(request).ConfigureAwait(false);
                return Success("User registered successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return Error("An error occurred during registration.", 500); 
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return Error(ErrorCodes.ValidationError); 
            }

            try
            {
                var userResponse = await _userService.LoginAsync(request).ConfigureAwait(false);
                return Success(userResponse); 
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedError("Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return Error("An error occurred during login.", 500); 
            }
        }
    }
}