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
    public class MerchantsController : SecureController
    {
        private readonly IMerchantsService _service;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<MerchantsController> _logger;

        public MerchantsController(
            IMerchantsService service,
            IStringLocalizer<SharedResources> localizer,
            ILogger<MerchantsController> logger)
            : base(localizer)
        {
            _service = service;
            _logger = logger;
            _localizer = localizer;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantRequest dto)
        {
            try
            {
                await _service.CreateMerchantAsync(dto).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantCreatedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating merchant with Name_Ar: {Name_Ar}, Name_En: {Name_En}", dto.Name_Ar, dto.Name_En);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantCreationError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMerchants()
        {
            try
            {
                var result = await _service.GetActiveMerchantsAsync().ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("ActiveMerchantsFetchedSuccessfully", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active merchants.");
                var localizedError = LocalizationHelper.GetLocalizedString("ActiveMerchantsFetchError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("by-group/{groupId}")]
        public async Task<IActionResult> GetMerchantsByGroupId(int groupId)
        {
            try
            {
                var result = await _service.GetMerchantsByGroupIdAsync(groupId).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantsFetchedByGroupId", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchants by group id: {GroupId}", groupId);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantsFetchByGroupError", "en"); 
                return Error(string.Format(localizedError, groupId), 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMerchantDetailsById(int id)
        {
            try
            {
                var result = await _service.GetMerchantDetailsByIdAsync(id).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantDetailsFetched", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchant details for id: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantDetailsFetchError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }

        [HttpGet("{id}/with-branches")]
        public async Task<IActionResult> GetMerchantWithBranches(int id)
        {
            try
            {
                var result = await _service.GetMerchantWithBranchesAsync(id).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantWithBranchesFetched", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching merchant with branches for id: {Id}", id);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantBranchesFetchError", "en");
                return Error(string.Format(localizedError, id), 500);
            }
        }

        [HttpGet("{merchantId}/main-branch")]
        public async Task<IActionResult> GetMerchantMainBranch(int merchantId)
        {
            try
            {
                var result = await _service.GetMerchantMainBranchAsync(merchantId).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MainBranchFetched", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching main branch for merchant id: {MerchantId}", merchantId);
                var localizedError = LocalizationHelper.GetLocalizedString("MainBranchFetchError", "en");
                return Error(string.Format(localizedError, merchantId), 500);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMerchants(
        [FromQuery] string name,
        [FromQuery] string mobile,
        [FromQuery] int? cityId,
        [FromQuery] string branchName)
        {
            try
            {
                var result = await _service.SearchMerchantsAsync(name, mobile, cityId ?? 0, branchName).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantsSearchResults", "en");
                return Success(localizedMessage, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching merchants.");
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantsSearchError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("search/pdf")]
        public async Task<IActionResult> SearchMerchantsAsPdf(
            [FromQuery] string name,
            [FromQuery] string mobile,
            [FromQuery] int cityId,
            [FromQuery] string branchName)
        {
            try
            {
                var htmlContent = await _service.GenerateSearchResultsHtmlAsync(name, mobile, cityId, branchName);
                var pdfBytes = await PdfGenerator.GeneratePdfAsync(htmlContent);

                return File(pdfBytes, "application/pdf", "MerchantsSearchResults.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for merchants search.");
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantsSearchPdfError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpGet("search-with-branches/pdf")]
        public async Task<IActionResult> SearchMerchantsWithBranchesAsPdf(
            [FromQuery] string name,
            [FromQuery] string mobile,
            [FromQuery] int cityId,
            [FromQuery] string branchName)
        {
            try
            {
                var htmlContent = await _service.GenerateSearchWithBranchesResultsHtmlAsync(name, mobile, cityId, branchName);
                var pdfBytes = await PdfGenerator.GeneratePdfAsync(htmlContent);

                return File(pdfBytes, "application/pdf", "MerchantsWithBranchesSearchResults.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for merchants search with branches.");
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantsWithBranchesSearchPdfError", "en");
                return Error(localizedError, 500);
            }
        }

        [HttpPut("change-group/{merchantId}")]
        public async Task<IActionResult> ChangeMerchantGroupId(int merchantId, [FromQuery] int newGroupId)
        {
            try
            {
                await _service.ChangeMerchantGroupIdAsync(merchantId, newGroupId).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantGroupChangedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing merchant group for merchant id: {MerchantId}", merchantId);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantGroupChangeError", "en");
                return Error(string.Format(localizedError), 500);
            }
        }

        [HttpPut("update/{merchantId}")]
        public async Task<IActionResult> UpdateMerchantDetails(int merchantId, [FromBody] MerchantRequest dto)
        {
            try
            {
                await _service.UpdateMerchantDetailsAsync(merchantId, dto).ConfigureAwait(false);
                var localizedMessage = LocalizationHelper.GetLocalizedString("MerchantUpdatedSuccessfully", "en");
                return Success(localizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating merchant details for merchant id: {MerchantId}", merchantId);
                var localizedError = LocalizationHelper.GetLocalizedString("MerchantUpdateError", "en");
                return Error(string.Format(localizedError, merchantId), 500);
            }
        }
    }
}