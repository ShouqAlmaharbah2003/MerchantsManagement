using DataLib.Models;

namespace DataLib.Interfaces
{
    public interface IMerchantsRepository : IRepository<Merchant>
    {
        Task<IList<Merchant>> GetActiveMerchantsAsync();
        Task<IList<Merchant>> GetByGroupIdAsync(int groupId);
        Task<Merchant> GetMerchantWithBranchesAsync(int id);
        Task<MerchantBranch> GetMainBranchAsync(int merchantId);
        Task<IEnumerable<Merchant>> GetMerchantsByGroupIdAsync(int groupId);
    }
}