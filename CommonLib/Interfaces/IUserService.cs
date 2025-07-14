using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using DataLib.Models;
using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    public interface IUserService
    {
        Task<UserWithTokenResponse> LoginAsync(LoginRequest dto);
        Task<User> RegisterAsync(RegisterRequest dto);
    }
}