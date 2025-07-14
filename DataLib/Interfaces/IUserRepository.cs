using DataLib.Models;
using System.Threading.Tasks;

namespace DataLib.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
    }
}