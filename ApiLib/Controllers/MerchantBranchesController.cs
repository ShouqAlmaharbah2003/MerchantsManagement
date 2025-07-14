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
//    public class MerchantBranchesController : SecureController
//    {
//        private readonly IMerchantBranchesService _service;
//        private readonly IStringLocalizer<MerchantBranchesController> _localizer;
//        private readonly ILogger<MerchantBranchesController> _logger;

//        public MerchantBranchesController(IMerchantBranchesService service, IStringLocalizer<MerchantBranchesController> localizer, ILogger<MerchantBranchesController> logger) : base(localizer)
//        {
//            _service = service;
//            _localizer = localizer;//_localizer[""]
//            _logger = logger;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreateBranch([FromBody] MerchantBranchRequest dto)
//        {
//            try
//            {
//                await _service.CreateBranchAsync(dto).ConfigureAwait(false);
//                return SuccessResponse(_localizer["Branchcreatedsuccessfully"]);// _localizer["Branch created successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["Errorcreatingbranch"]);// _localizer["Error creating branch"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpGet("all")]
//        public async Task<IActionResult> GetAllBranches()
//        {
//            try
//            {
//                var result = await _service.GetAllBranchesAsync().ConfigureAwait(false);
//                return SuccessResponse(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["ErrorFetchingAllBranches"]);//_localizer["Error Fetching All Branches"]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpPut("update/{id}")]
//        public async Task<IActionResult> UpdateBranch(int id, [FromBody] MerchantBranchRequest dto)
//        {
//            try
//            {
//                await _service.UpdateBranchAsync(id, dto).ConfigureAwait(false);
//                return SuccessResponse(_localizer["Branchupdatedsuccessfully"]);// _localizer["Branch updated successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,_localizer["Errorupdatingbranchwithid:{Id}", id]);// _localizer["Error updating branch with id: {Id}", id]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }

//        [HttpDelete("delete/{id}")]
//        public async Task<IActionResult> DeleteBranch(int id)
//        {
//            try
//            {
//                await _service.DeleteBranchAsync(id).ConfigureAwait(false);
//                return SuccessResponse(_localizer["Branchdeletedsuccessfully"]);// _localizer["Branch deleted successfully."]
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, _localizer["Errordeletingbranchwithid:{Id}", id]);// _localizer["Error deleting branch with id: {Id}", id]
//                return StatusCode(500, ErrorResponse(CommonLib.Enums.ErrorCodes.InternalServerError));
//            }
//        }
//    }
//}