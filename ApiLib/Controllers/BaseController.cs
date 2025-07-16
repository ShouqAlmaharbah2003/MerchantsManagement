using CommonLib.Dtos.Responses;
using CommonLib.Enums;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;
        protected BaseController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        protected IActionResult SuccessResponse<T>(T data)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data
            };
            return Ok(response);
        }

        protected IActionResult ErrorResponse(ErrorCodes errorCode)
        {
            var localizedMessage = _localizationService.GetLocalizedString(errorCode.ToString());
            var response = new ApiResponse<object>
            {
                Success = false,
                ErrorCode = (int)errorCode,
                ErrorMessage = localizedMessage
            };
            return BadRequest(response);
        }
    }
}