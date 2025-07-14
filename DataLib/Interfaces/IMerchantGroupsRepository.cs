using DataLib.Models;

namespace DataLib.Interfaces
{
    public interface IMerchantGroupsRepository : IRepository<MerchantGroup>
    {
        Task<IList<MerchantGroup>> GetActiveGroupsAsync();
    }
}