using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    public interface IMerchantGroupsService
    {
        Task<List<MerchantGroupResponse>> GetAllGroupsAsync();
        Task<List<MerchantGroupResponse>> GetActiveGroupsAsync();
        Task CreateGroupAsync(MerchantGroupRequest dto);
        Task UpdateGroupName_ArAsync(int groupId, string newName);
        Task UpdateGroupName_EnAsync(int groupId, string newName);
        Task DeleteGroupAsync(int groupId);
    }
}