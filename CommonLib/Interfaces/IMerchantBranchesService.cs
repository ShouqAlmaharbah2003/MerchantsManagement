using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    public interface IMerchantBranchesService
    {
        Task<List<MerchantBranchResponse>> GetAllBranchesAsync();
        Task CreateBranchAsync(MerchantBranchRequest dto);
        Task UpdateBranchAsync(int branchId, MerchantBranchRequest dto);
        Task DeleteBranchAsync(int branchId);
    }
}