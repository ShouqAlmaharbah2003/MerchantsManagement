using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(string username);
        Task<(bool IsValid, string Username)> ValidateTokenAsync(string token);
    }
}