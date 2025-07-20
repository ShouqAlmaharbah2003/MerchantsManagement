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
    public class MerchantBranchesController : SecureController
    {
        private readonly IMerchantBranchesService _service;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ILogger<MerchantBranchesController> _logger;
        public MerchantBranchesController(
            IMerchantBranchesService service,
            IStringLocalizer<Resources> localizer,
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
                return Success(_localizer["BranchCreatedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch.");
                return Error(_localizer["BranchCreationError"].Value, 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var result = await _service.GetAllBranchesAsync().ConfigureAwait(false);
                return Success(_localizer["BranchesFetchedSuccessfully"].Value, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all branches.");
                return Error(_localizer["BranchesFetchError"].Value, 500);
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
                return Success(_localizer["BranchUpdatedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating branch with ID: {Id}", id);
                return Error(string.Format(_localizer["BranchUpdateError"].Value, id), 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                await _service.DeleteBranchAsync(id).ConfigureAwait(false);
                return Success(_localizer["BranchDeletedSuccessfully"].Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting branch with ID: {Id}", id);
                return Error(string.Format(_localizer["BranchDeletionError"].Value, id), 500);
            }
        }
    }
}