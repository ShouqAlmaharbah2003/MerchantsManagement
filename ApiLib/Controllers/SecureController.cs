using ApiLib.Filters;
using CommonLib.Enums;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizationFilter))]
    [ServiceFilter(typeof(LocalizationFilter))]
    public abstract class SecureController : BaseController
    {
        protected SecureController(ILocalizationService localizationService)
            : base(localizationService)
        {
        }
        protected IActionResult Success(object data)
        {
            return SuccessResponse(data);
        }

        protected IActionResult Success(string message)
        {
            return SuccessResponse(new { Message = message });
        }

        protected IActionResult Error(ErrorCodes errorCode)
        {
            return ErrorResponse(errorCode);
        }

        protected IActionResult Error(string errorMessage, int errorCode = 400)
        {
            return BadRequest(new
            {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            });
        }

        protected IActionResult NotFoundError(string message)
        {
            return NotFound(new
            {
                Success = false,
                ErrorMessage = message
            });
        }

        protected IActionResult UnauthorizedError(string message)
        {
            return Unauthorized(new
            {
                Success = false,
                ErrorMessage = message
            });
        }

        protected bool IsArabic =>
            Request.Headers["Accept-Language"].ToString().StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        protected bool IsEnglish =>
            Request.Headers["Accept-Language"].ToString().StartsWith("en", StringComparison.OrdinalIgnoreCase);

    }
}