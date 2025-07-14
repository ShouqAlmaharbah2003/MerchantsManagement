using DataLib.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLib.Interfaces
{
    public interface IMerchantBranchesRepository : IRepository<MerchantBranch>
    {
        Task<IEnumerable<MerchantBranch>> GetBranchesByMerchantIdAsync(int id);
        Task<MerchantBranch> GetMainBranchByMerchantIdAsync(int merchantId);
    }
}