using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using DataLib.Models;

namespace CommonLib.Interfaces
{
    public interface IUserService
    {
        Task<UserWithTokenResponse> LoginAsync(LoginRequest dto);
        Task<User> RegisterAsync(RegisterRequest dto);
    }
}