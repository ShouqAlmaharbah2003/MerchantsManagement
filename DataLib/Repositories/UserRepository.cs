using DataLib.Models;
using DataLib.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace DataLib.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISession _session;

        public UserRepository(ISession session)
        {
            _session = session;
        }

        public async Task<User> GetByIdAsync(int id)
        {
                var user = await _session.GetAsync<User>(id).ConfigureAwait(false);
                return user;
        }

        public async Task<IList<User>> GetAllAsync()
        {
                var users = await _session.Query<User>().ToListAsync().ConfigureAwait(false);
                return users;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

                var user = await _session.Query<User>()
                    .FirstOrDefaultAsync(u => u.Username == username).ConfigureAwait(false);
                return user;
        }

        public async Task SaveAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                await _session.SaveAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                await _session.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task DeleteAsync(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                entity.DeletedAt = DateTime.UtcNow;
                await _session.UpdateAsync(entity).ConfigureAwait(false);
        }
    }
}