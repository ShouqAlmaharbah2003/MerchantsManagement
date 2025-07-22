using CommonLib.Dtos.Requests;
using CommonLib.Enums;
using CommonLib.Interfaces;
using CommonLib.Resources;
using CommonLib.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : SecureController
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IUserService userService, ILogger<AuthController> logger,
            IStringLocalizer<SharedResources> localizer)
            : base(localizer)
        {
            _userService = userService;
            _localizer = localizer;
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
                return ErrorResponse(ErrorCodes.ValidationError); 
            }

            try
            {
                var user = await _userService.RegisterAsync(request).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("Userregisteredsuccessfully", "en");
                
                return Success(localizedMessage, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User registration failed: {Message}", ex.Message);
                var localizedError = LocalizationHelper.GetLocalizedString("UserAlreadyExists", "en");
                return Error(localizedError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                var localizedError = LocalizationHelper.GetLocalizedString("RegistrationError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return ErrorResponse(ErrorCodes.ValidationError); 
            }

            try
            {
                var userResponse = await _userService.LoginAsync(request).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("LoginSuccessful", "en");

                return Success(localizedMessage, userResponse);
            }
            catch (UnauthorizedAccessException)
            {
                var localizedError = LocalizationHelper.GetLocalizedString("InvalidUsernameOrPassword", "en");
                return UnauthorizedError(localizedError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                var localizedError = LocalizationHelper.GetLocalizedString("LoginError", "en");
                return Error(localizedError, 500);
            }
        }
    }
}