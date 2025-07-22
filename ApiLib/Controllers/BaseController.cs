using CommonLib.Dtos.Responses;
using CommonLib.Enums;
using CommonLib.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ApiLib.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        protected BaseController(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        protected IActionResult SuccessResponse()
        {
            var response = new ApiResponse<object>
            {
                Success = true
            };
            return Ok(response);
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
            var localizedMessage = _localizer[errorCode.ToString()];
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