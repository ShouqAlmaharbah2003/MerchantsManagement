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



















//using CommonLib.Enums;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Localization;

//namespace ApiLib.Controllers
//{
//    [ApiController]
//    public class BaseController : ControllerBase// make these methods static 
//                                                // or abstract if you want to prevent instantiation
//                                                // or you can use a base class for all controllers

//    {
//        private readonly IStringLocalizer _localizer;

//        public BaseController(IStringLocalizer localizer)
//        {
//            _localizer = localizer;
//        }

//        protected IActionResult SuccessResponse(object data)
//        {
//            return Ok(new
//            {
//                Success = true,
//                Data = data
//            });
//        }

//        protected IActionResult ErrorResponse(ErrorCodes errorCode)
//        {
//            return BadRequest(new
//            {
//                Success = false,
//                ErrorCode = errorCode,
//                ErrorMessage = _localizer[errorCode.ToString()]
//            });
//        }
//    }
//}