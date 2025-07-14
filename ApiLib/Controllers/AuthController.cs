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



















//using CommonLib.Dtos.Requests;
//using CommonLib.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Localization;

//namespace ApiLib.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly IStringLocalizer<AuthController> _localizer;



//        public AuthController(IUserService userService, IStringLocalizer<AuthController> localizer)
//        {
//            _userService = userService;
//            _localizer = localizer;//_localizer[""]
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
//        {
//            try
//            {
//                if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Email))
//                {
//                    return BadRequest(_localizer["UsernamePasswordAndEmailRequired"]);// _localizer["Username, Password, and Email are required."]);
//                }

//                var user = await _userService.RegisterAsync(request).ConfigureAwait(false);

//                return Ok(_localizer["Userregisteredsuccessfully"]);// _localizer["User registered successfully, Username = user.Username"]);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(_localizer["Registrationfailed.Reason: {Message}"]);// _localizer["Registration failed. Reason: {Message}"]);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, _localizer["Anerroroccurredduringlogin"]);// _localizer["An error occurred during registration."]);
//            }
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            try
//            {
//                if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
//                {
//                    return BadRequest(_localizer["UsernameandPasswordarerequired"]);// _localizer["Username and Password are required."]);
//                }

//                var userResponse = await _userService.LoginAsync(request).ConfigureAwait(false);

//                return Ok(userResponse);
//            }
//            catch (UnauthorizedAccessException)
//            {
//                return Unauthorized(_localizer["Invalidusernameorpassword"]);// _localizer["Invalid username or password."]);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, _localizer["Anerroroccurredduringlogin"]);// _localizer["An error occurred during login."]);
//            }
//        }
//    }
//}