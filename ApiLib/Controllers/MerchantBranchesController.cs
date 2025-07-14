using CommonLib.Dtos.Requests;
using CommonLib.Enums;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MerchantBranchesController : SecureController
    {
        private readonly IMerchantBranchesService _service;
        private readonly ILogger<MerchantBranchesController> _logger;

        public MerchantBranchesController(
            IMerchantBranchesService service,
            ILocalizationService localizationService,
            ILogger<MerchantBranchesController> logger)
            : base(localizationService)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBranch([FromBody] MerchantBranchRequest dto)
        {
            if (dto == null)
                return Error(ErrorCodes.ValidationError);

            try
            {
                await _service.CreateBranchAsync(dto).ConfigureAwait(false);
                return Success("Branch created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch.");
                return Error("An error occurred while creating the branch.", 500);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var result = await _service.GetAllBranchesAsync().ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all branches.");
                return Error("An error occurred while fetching branches.", 500);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] MerchantBranchRequest dto)
        {
            if (dto == null)
                return Error(ErrorCodes.ValidationError);

            try
            {
                await _service.UpdateBranchAsync(id, dto).ConfigureAwait(false);
                return Success("Branch updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating branch with ID: {Id}", id);
                return Error($"An error occurred while updating branch with ID: {id}", 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                await _service.DeleteBranchAsync(id).ConfigureAwait(false);
                return Success("Branch deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting branch with ID: {Id}", id);
                return Error($"An error occurred while deleting branch with ID: {id}", 500);
            }
        }
    }
}