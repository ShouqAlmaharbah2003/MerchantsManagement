using DataLib.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLib.Interfaces
{
    public interface IMerchantGroupsRepository : IRepository<MerchantGroup>
    {
        Task<IList<MerchantGroup>> GetActiveGroupsAsync();
    }
}