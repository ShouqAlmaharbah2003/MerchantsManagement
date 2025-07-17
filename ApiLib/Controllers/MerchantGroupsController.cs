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
    [Authorize]
    public class MerchantGroupsController : SecureController
    {
        private readonly IMerchantGroupsService _service;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ILogger<MerchantGroupsController> _logger;
        public MerchantGroupsController(
            IMerchantGroupsService service,
            ILocalizationService localizationService,
            IStringLocalizer<Resources> localizer,
            ILogger<MerchantGroupsController> logger)
            : base(localizationService)
        {
            _service = service;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] MerchantGroupRequest dto)
        {
            if (dto == null)
                return ErrorResponse(ErrorCodes.ValidationError);

            try
            {
                await _service.CreateGroupAsync(dto).ConfigureAwait(false);
                return Success(_localizer["GroupCreatedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group.");
                return Error(_localizer["GroupCreationError"].Value, 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var result = await _service.GetAllGroupsAsync().ConfigureAwait(false);
                return Success(_localizer["GroupsFetchedSuccessfully"].Value, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all groups.");
                return Error(_localizer["GroupsFetchError"].Value, 500);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGroups()
        {
            try
            {
                var result = await _service.GetActiveGroupsAsync().ConfigureAwait(false);
                return Success(_localizer["ActiveGroupsFetchedSuccessfully"].Value, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active groups.");
                return Error(_localizer["ActiveGroupsFetchError"].Value, 500);
            }
        }

        [HttpPut("updateName/{id}")]
        public async Task<IActionResult> UpdateGroupName(int id, [FromQuery] string newName, [FromQuery] string language)
        {
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(language))
                return ErrorResponse(ErrorCodes.ValidationError);

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
                    return Error(_localizer["InvalidLanguage"].Value, 400);
                }

                return Success(_localizer["GroupNameUpdatedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group name for ID: {Id}", id);
                return Error(string.Format(_localizer["GroupNameUpdateError"].Value, id), 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                await _service.DeleteGroupAsync(id).ConfigureAwait(false);
                return Success(_localizer["GroupDeletedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group with ID: {Id}", id);
                return Error(string.Format(_localizer["GroupDeletionError"].Value, id), 500);
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
//    [Authorize]
//    public class MerchantGroupsController : SecureController
//    {
//        private readonly IMerchantGroupsService _service;
//        private readonly IStringLocalizer<Resources> _localizer;
//        private readonly ILogger<MerchantGroupsController> _logger;
//        public MerchantGroupsController(
//            IMerchantGroupsService service,
//            ILocalizationService localizationService,
//            IStringLocalizer<Resources> localizer,
//            ILogger<MerchantGroupsController> logger)
//            : base(localizationService)
//        {
//            _service = service;
//            _localizer = localizer;
//            _logger = logger;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreateGroup([FromBody] MerchantGroupRequest dto)
//        {
//            if (dto == null)
//                return Error(ErrorCodes.ValidationError);// _localizer["ValidationError"].Value);

//            try
//            {
//                await _service.CreateGroupAsync(dto).ConfigureAwait(false);
//                return Success("Group created successfully.");// _localizer["GroupCreatedSuccessfully"].Value);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating group.");
//                return Error("An error occurred while creating the group.", 500);// _localizer["GroupCreationError"].Value);
//            }
//        }

//        [HttpGet("all")]
//        public async Task<IActionResult> GetAllGroups()
//        {
//            try
//            {
//                var result = await _service.GetAllGroupsAsync().ConfigureAwait(false);
//                return Success(result);// _localizer["GroupsFetchedSuccessfully"].Value, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching all groups.");
//                return Error("An error occurred while fetching groups.", 500);// _localizer["GroupsFetchError"].Value);
//            }
//        }

//        [HttpGet("active")]
//        public async Task<IActionResult> GetActiveGroups()
//        {
//            try
//            {
//                var result = await _service.GetActiveGroupsAsync().ConfigureAwait(false);
//                return Success(result);// _localizer["ActiveGroupsFetchedSuccessfully"].Value, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching active groups.");
//                return Error("An error occurred while fetching active groups.", 500);// _localizer["ActiveGroupsFetchError"].Value);
//            }
//        }

//        [HttpPut("updateName/{id}")]
//        public async Task<IActionResult> UpdateGroupName(int id, [FromQuery] string newName, [FromQuery] string language)
//        {
//            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(language))
//                return Error(ErrorCodes.ValidationError);// _localizer["ValidationError"].Value);

//            try
//            {
//                if (language.Equals("ar", StringComparison.OrdinalIgnoreCase))
//                {
//                    await _service.UpdateGroupName_ArAsync(id, newName).ConfigureAwait(false);
//                }
//                else if (language.Equals("en", StringComparison.OrdinalIgnoreCase))
//                {
//                    await _service.UpdateGroupName_EnAsync(id, newName).ConfigureAwait(false);
//                }
//                else
//                {
//                    return Error("Invalid language specified. Use 'ar' or 'en'.", 400);// _localizer["InvalidLanguage"].Value);
//                }

//                return Success("Group name updated successfully.");// _localizer["GroupNameUpdatedSuccessfully"].Value);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating group name for ID: {Id}", id);
//                return Error($"An error occurred while updating group name for ID: {id}", 500);// _localizer["GroupNameUpdateError"].Value);
//            }
//        }

//        [HttpDelete("delete/{id}")]
//        public async Task<IActionResult> DeleteGroup(int id)
//        {
//            try
//            {
//                await _service.DeleteGroupAsync(id).ConfigureAwait(false);
//                return Success("Group deleted successfully.");// _localizer["GroupDeletedSuccessfully"].Value);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting group with ID: {Id}", id);
//                return Error($"An error occurred while deleting group with ID: {id}", 500);// _localizer["GroupDeletionError"].Value);
//            }
//        }
//    }
//}