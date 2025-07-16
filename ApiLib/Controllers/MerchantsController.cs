using CommonLib.Dtos.Requests;
using CommonLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiLib.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MerchantsController : SecureController
    {
        private readonly IMerchantsService _service;
        private readonly ILogger<MerchantsController> _logger;
        public MerchantsController(
            IMerchantsService service,
            ILocalizationService localizationService,
            ILogger<MerchantsController> logger)
            : base(localizationService)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantRequest dto)
        {
            try
            {
                await _service.CreateMerchantAsync(dto).ConfigureAwait(false);
                return Success("Merchant created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating merchant with Name_Ar: {Name_Ar}, Name_En: {Name_En}", dto.Name_Ar, dto.Name_En);
                return Error("An error occurred while creating the merchant.", 500);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMerchants()
        {
            try
            {
                var result = await _service.GetActiveMerchantsAsync().ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active merchants.");
                return Error("An error occurred while fetching active merchants.", 500);
            }
        }

        [HttpGet("by-group/{groupId}")]
        public async Task<IActionResult> GetMerchantsByGroupId(int groupId)
        {
            try
            {
                var result = await _service.GetMerchantsByGroupIdAsync(groupId).ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchants by group id: {GroupId}", groupId);
                return Error($"An error occurred while fetching merchants for group ID: {groupId}", 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchantDetailsById(int id)
        {
            try
            {
                var result = await _service.GetMerchantDetailsByIdAsync(id).ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchant details for id: {Id}", id);
                return Error($"An error occurred while fetching details for merchant ID: {id}", 500);
            }
        }

        [HttpGet("{id}/with-branches")]
        public async Task<IActionResult> GetMerchantWithBranches(int id)
        {
            try
            {
                var result = await _service.GetMerchantWithBranchesAsync(id).ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchant with branches for id: {Id}", id);
                return Error($"An error occurred while fetching merchant branches for ID: {id}", 500);
            }
        }

        [HttpGet("{merchantId}/main-branch")]
        public async Task<IActionResult> GetMerchantMainBranch(int merchantId)
        {
            try
            {
                var result = await _service.GetMerchantMainBranchAsync(merchantId).ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching main branch for merchant id: {MerchantId}", merchantId);
                return Error($"An error occurred while fetching main branch for merchant ID: {merchantId}", 500);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMerchants(
            [FromQuery] string name,
            [FromQuery] string mobile,
            [FromQuery] int cityId,
            [FromQuery] string branchName)
        {
            try
            {
                var result = await _service.SearchMerchantsAsync(name, mobile, cityId, branchName).ConfigureAwait(false);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching merchants with parameters: {Name}, {Mobile}, {CityId}, {BranchName}", name, mobile, cityId, branchName);
                return Error("An error occurred while searching merchants.", 500);
            }
        }

        //[HttpGet("search/pdf")]//500
        //public async Task<IActionResult> SearchMerchantsPdf(
        //    [FromQuery] string name,
        //    [FromQuery] string mobile,
        //    [FromQuery] int cityId,
        //    [FromQuery] string branchName)
        //{
        //    try
        //    {
        //        var htmlContent = await _service.GenerateSearchResultsHtmlAsync(name, mobile, cityId, branchName).ConfigureAwait(false);
        //        var pdfBytes = await PdfGenerator.GeneratePdfAsync(htmlContent).ConfigureAwait(false);
        //        return File(pdfBytes, "application/pdf", "merchants_search_results.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generating PDF for search with parameters: {Name}, {Mobile}, {CityId}, {BranchName}", name, mobile, cityId, branchName);
        //        return Error("An error occurred while generating the PDF.", 500);
        //    }
        //}

        //[HttpGet("search/with-branches/pdf")]//500
        //public async Task<IActionResult> SearchMerchantsWithBranchesPdf(
        //    [FromQuery] string name,
        //    [FromQuery] string mobile,
        //    [FromQuery] int cityId,
        //    [FromQuery] string branchName)
        //{
        //    try
        //    {
        //        var htmlContent = await _service.GenerateSearchWithBranchesResultsHtmlAsync(name, mobile, cityId, branchName).ConfigureAwait(false);
        //        var pdfBytes = await PdfGenerator.GeneratePdfAsync(htmlContent).ConfigureAwait(false);
        //        return File(pdfBytes, "application/pdf", "merchants_with_branches_search_results.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generating PDF with branches for search with parameters: {Name}, {Mobile}, {CityId}, {BranchName}", name, mobile, cityId, branchName);
        //        return Error("An error occurred while generating the PDF with branches.", 500);
        //    }
        //}

        [HttpPut("change-group/{merchantId}")]
        public async Task<IActionResult> ChangeMerchantGroupId(int merchantId, [FromQuery] int newGroupId)
        {
            try
            {
                await _service.ChangeMerchantGroupIdAsync(merchantId, newGroupId).ConfigureAwait(false);
                return Success("Merchant group changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing merchant group for merchant id: {MerchantId}", merchantId);
                return Error($"An error occurred while changing merchant group for ID: {merchantId}", 500);
            }
        }

        [HttpPut("update/{merchantId}")]
        public async Task<IActionResult> UpdateMerchantDetails(int merchantId, [FromBody] MerchantRequest dto)
        {
            try
            {
                await _service.UpdateMerchantDetailsAsync(merchantId, dto).ConfigureAwait(false);
                return Success("Merchant updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating merchant details for merchant id: {MerchantId}", merchantId);
                return Error($"An error occurred while updating merchant details for ID: {merchantId}", 500);
            }
        }
    }
}