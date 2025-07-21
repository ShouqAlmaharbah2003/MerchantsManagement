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
    public class MerchantBranchesController : SecureController
    {
        private readonly IMerchantBranchesService _service;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<MerchantBranchesController> _logger;
        public MerchantBranchesController(
            IMerchantBranchesService service,
            IStringLocalizer<SharedResources> localizer,
            ILogger<MerchantBranchesController> logger)
            : base(localizer)
        {
            _service = service;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBranch([FromBody] MerchantBranchRequest dto)
        {
            if (dto == null)
                return ErrorResponse(ErrorCodes.ValidationError);

            try
            {
                await _service.CreateBranchAsync(dto).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("BranchCreatedSuccessfully", "en");

                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch.");
                var localizedError = LocalizationHelper.GetLocalizedString("BranchCreationError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var result = await _service.GetAllBranchesAsync().ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("BranchesFetchedSuccessfully", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all branches.");
                var localizedError = LocalizationHelper.GetLocalizedString("BranchesFetchError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] MerchantBranchRequest dto)
        {
            if (dto == null)
                return ErrorResponse(ErrorCodes.ValidationError);

            try
            {
                await _service.UpdateBranchAsync(id, dto).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("BranchUpdatedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating branch with ID: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("BranchUpdateError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                await _service.DeleteBranchAsync(id).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("BranchDeletedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting branch with ID: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("BranchDeletionError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }
    }
}