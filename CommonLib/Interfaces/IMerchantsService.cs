using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;

namespace CommonLib.Interfaces
{
    public interface IMerchantsService
    {
        Task CreateMerchantAsync(MerchantRequest dto);
        Task ChangeMerchantGroupIdAsync(int merchantId, int newGroupId);
        Task<string> GenerateSearchResultsHtmlAsync(string name, string mobile, int cityId, string branchName);
        Task<string> GenerateSearchWithBranchesResultsHtmlAsync(string name, string mobile, int cityId, string branchName);
        Task<List<MerchantResponse>> GetActiveMerchantsAsync();
        Task<MerchantResponse> GetMerchantDetailsByIdAsync(int id);
        Task<MerchantBranchResponse> GetMerchantMainBranchAsync(int merchantId);
        Task<List<MerchantResponse>> GetMerchantsByGroupIdAsync(int groupId);
        Task<MerchantWithBranchesResponse> GetMerchantWithBranchesAsync(int id);
        Task<List<MerchantResponse>> SearchMerchantsAsync(string name, string mobile, int cityId, string branchName);
        Task UpdateMerchantDetailsAsync(int merchantId, MerchantRequest dto);
        byte[] ConvertToPDF(string htmlContent);
    }
}