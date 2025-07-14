using CommonLib.Dtos.Requests;
using CommonLib.Enums;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MerchantGroupsController : SecureController
    {
        private readonly IMerchantGroupsService _service;
        private readonly ILogger<MerchantGroupsController> _logger;

        public MerchantGroupsController(
            IMerchantGroupsService service,
            ILocalizationService localizationService,
            ILogger<MerchantGroupsController> logger)
            : base(localizationService)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] MerchantGroupRequest dto)
        {
            if (dto == null)
                return Error(ErrorCodes.ValidationError);

            try
            {
                await _service.CreateGroupAsync(dto).ConfigureAwait(false);
                return Success("Group created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group.");
                return Error("An error occurred while creating the group.", 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var result = await _service.GetAllGroupsAsync().ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all groups.");
                return Error("An error occurred while fetching groups.", 500);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGroups()
        {
            try
            {
                var result = await _service.GetActiveGroupsAsync().ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active groups.");
                return Error("An error occurred while fetching active groups.", 500);
            }
        }

        [HttpPut("updateName/{id}")]
        public async Task<IActionResult> UpdateGroupName(int id, [FromQuery] string newName, [FromQuery] string language)
        {
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(language))
                return Error(ErrorCodes.ValidationError);

            try
            {
                if (language.Equals("ar", StringComparison.OrdinalIgnoreCase))
                {
                    await _service.UpdateGroupName_ArAsync(id, newName).ConfigureAwait(false);
                }
                else if (language.Equals("en", StringComparison.OrdinalIgnoreCase))
                {
                    await _service.UpdateGroupName_EnAsync(id, newName).ConfigureAwait(false);
                }
                else
                {
                    return Error("Invalid language specified. Use 'ar' or 'en'.", 400);
                }

                return Success("Group name updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group name for ID: {Id}", id);
                return Error($"An error occurred while updating group name for ID: {id}", 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                await _service.DeleteGroupAsync(id).ConfigureAwait(false);
                return Success("Group deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group with ID: {Id}", id);
                return Error($"An error occurred while deleting group with ID: {id}", 500);
            }
        }
    }
}




























//using CommonLib.Dtos.Requests;
//using CommonLib.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Localization;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;

//namespace ApiLib.Controllers
//{
//    [Route("api/[controller]")]
//    [Authorize]
//    public class MerchantGroupsController : SecureController
//    {
//        private readonly IMerchantGroupsService _service;
//        private readonly IStringLocalizer<MerchantGroupsController> _localizer;
//        private readonly ILogger<MerchantGroupsController> _logger;

//        public MerchantGroupsController(IMerchantGroupsService service, IStringLocalizer<MerchantGroupsController> localizer, ILogger<MerchantGroupsController> logger) : base(localizer)
//        {
//            _service = service;
//            _localizer = localizer; //_localizer[""]
//            _logger = logger;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreateGroup([FromBody] MerchantGroupRequest dto)
//        {
//            try
//            {
//                await _service.CreateGroupAsync(dto).ConfigureAwait(false);
//                return SuccessResponse(_localizer["GroupCreatedSuccessfully"]);// _localizer["Group created successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorCreatingGroup"]);// _localizer["Error creating group"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpGet("all")]
//        public async Task<IActionResult> GetAllGroups()
//        {
//            try
//            {
//                var result = await _service.GetAllGroupsAsync().ConfigureAwait(false);
//                return SuccessResponse(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorFetchingAllGroups"]);// _localizer["Error Fetching All Groups"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpGet("active")]
//        public async Task<IActionResult> GetActiveGroups()
//        {
//            try
//            {
//                var result = await _service.GetActiveGroupsAsync().ConfigureAwait(false);
//                return SuccessResponse(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorFetchingActiveGroups"]);// _localizer["Error Fetching Active Groups"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }
//        // make them one endpoint with a query parameter for language

//        [HttpPut("updateAr/{id}")]
//        public async Task<IActionResult> UpdateGroupName_Ar(int id, [FromQuery] string newName)
//        {
//            try
//            {
//                await _service.UpdateGroupName_ArAsync(id, newName).ConfigureAwait(false);
//                return SuccessResponse(_localizer["GroupUpdatedSuccessfully"]);// _localizer["Group updated successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorUpdatingGroupNameAr"], id);// _localizer["Error updating group name (Arabic) for ID: {id}"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpPut("updateEn/{id}")]
//        public async Task<IActionResult> UpdateGroupName_En(int id, [FromQuery] string newName)
//        {
//            try
//            {
//                await _service.UpdateGroupName_EnAsync(id, newName).ConfigureAwait(false);
//                return SuccessResponse(_localizer["GroupUpdatedSuccessfully"]);// _localizer["Group updated successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorUpdatingGroupNameEn"], id);// _localizer["Error updating group name (English) for ID: {id}"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpDelete("delete/{id}")]
//        public async Task<IActionResult> DeleteGroup(int id)
//        {
//            try
//            {
//                await _service.DeleteGroupAsync(id).ConfigureAwait(false);
//                return SuccessResponse(_localizer["GroupDeletedSuccessfully"]);// _localizer["Group deleted successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorDeletingGroup"], id);// _localizer["Error deleting group with ID: {id}"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }
//    }
//}