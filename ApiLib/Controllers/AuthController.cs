using CommonLib.Dtos.Requests;
using CommonLib.Enums;
using CommonLib.Interfaces;
using CommonLib.Resources;
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
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IUserService userService, ILogger<AuthController> logger,
            IStringLocalizer<Resources> localizer,
            ILocalizationService localizationService)
            : base(localizationService)
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
                return ErrorResponse(ErrorCodes.ValidationError); // Uses _localizationService
            }

            try
            {
                var user = await _userService.RegisterAsync(request).ConfigureAwait(false);
                return Success(_localizer["UserRegisteredSuccessfully"].Value, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User registration failed: {Message}", ex.Message);
                return Error(_localizer["UserAlreadyExists"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return Error(_localizer["RegistrationError"].Value, 500);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return ErrorResponse(ErrorCodes.ValidationError); // Uses _localizationService
            }

            try
            {
                var userResponse = await _userService.LoginAsync(request).ConfigureAwait(false);
                return Success(_localizer["LoginSuccessful"].Value, userResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return UnauthorizedError(_localizer["InvalidUsernameOrPassword"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return Error(_localizer["LoginError"].Value, 500);
            }
        }
    }
}




//using CommonLib.Dtos.Requests;
//using CommonLib.Enums;
//using CommonLib.Interfaces;
//using CommonLib.Resources;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Localization;

//namespace ApiLib.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class AuthController : SecureController
//    {
//        private readonly IUserService _userService;
//        private readonly IStringLocalizer<Resources> _localizer;
//        private readonly ILogger<AuthController> _logger;
//        public AuthController(
//            IUserService userService, ILogger<AuthController> logger,
//            IStringLocalizer<Resources> localizer,
//            ILocalizationService localizationService)
//            : base(localizationService)
//        {
//            _userService = userService;
//            _localizer = localizer;
//            _logger = logger;
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
//                string.IsNullOrWhiteSpace(request.Password) ||
//                string.IsNullOrWhiteSpace(request.Email))
//            {
//                return Error(ErrorCodes.ValidationError); // _localizer["ValidationError"].Value);
//            }

//            try
//            {
//                var user = await _userService.RegisterAsync(request).ConfigureAwait(false);
//                return Success("User registered successfully.");// _localizer["UserRegisteredSuccessfully"].Value, user);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return Error(ex.Message); // _localizer["UserAlreadyExists"].Value);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred during registration.");
//                return Error("An error occurred during registration.", 500); // _localizer["RegistrationError"].Value);
//            }
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Username) ||
//                string.IsNullOrWhiteSpace(request.Password))
//            {
//                return Error(ErrorCodes.ValidationError); // _localizer["ValidationError"].Value);
//            }

//            try
//            {
//                var userResponse = await _userService.LoginAsync(request).ConfigureAwait(false);
//                return Success(userResponse); // _localizer["LoginSuccessful"].Value, userResponse);
//            }
//            catch (UnauthorizedAccessException)
//            {
//                return UnauthorizedError("Invalid username or password."); // _localizer["InvalidUsernameOrPassword"].Value);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred during login.");
//                return Error("An error occurred during login.", 500); // _localizer["LoginError"].Value);
//            }
//        }
//    }
//}