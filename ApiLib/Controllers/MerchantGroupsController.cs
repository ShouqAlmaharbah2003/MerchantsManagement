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
    [Authorize]
    public class MerchantGroupsController : SecureController
    {
        private readonly IMerchantGroupsService _service;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<MerchantGroupsController> _logger;
        public MerchantGroupsController(
            IMerchantGroupsService service,
            IStringLocalizer<SharedResources> localizer,
            ILogger<MerchantGroupsController> logger)
            : base(localizer)
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
                var localizedMessage = LocalizationHelper.GetLocalizedString("GroupCreatedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group.");
                var localizedError = LocalizationHelper.GetLocalizedString("GroupCreationError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var result = await _service.GetAllGroupsAsync().ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("GroupsFetchedSuccessfully", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all groups.");
                var localizedError = LocalizationHelper.GetLocalizedString("GroupsFetchError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGroups()
        {
            try
            {
                var result = await _service.GetActiveGroupsAsync().ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("ActiveGroupsFetchedSuccessfully", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active groups.");
                var localizedError = LocalizationHelper.GetLocalizedString("ActiveGroupsFetchError", "en");
                return Error(localizedError, 500);
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
                    var localizedError = LocalizationHelper.GetLocalizedString("InvalidLanguage", "en");
                    return Error(localizedError, 400);
                }
                var localizedMessage = LocalizationHelper.GetLocalizedString("GroupNameUpdatedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group name for ID: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("GroupNameUpdateError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                await _service.DeleteGroupAsync(id).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("GroupDeletedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group with ID: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("GroupDeletionError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }
    }
}