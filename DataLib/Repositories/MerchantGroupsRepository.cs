using DataLib.Models;
using DataLib.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace DataLib.Repositories
{
    public class MerchantGroupsRepository : IMerchantGroupsRepository
    {
        private readonly ISession _session;

        public MerchantGroupsRepository(ISession session)
        {
            _session = session;
        }

        public async Task<MerchantGroup> GetByIdAsync(int id)
        {
                var group = await _session.GetAsync<MerchantGroup>(id).ConfigureAwait(false);
                return group;
        }

        public async Task<IList<MerchantGroup>> GetAllAsync()
        {
                var groups = await _session.Query<MerchantGroup>().ToListAsync().ConfigureAwait(false);
                return groups;
        }

        public async Task<IList<MerchantGroup>> GetActiveGroupsAsync()
        {
                var groups = await _session.Query<MerchantGroup>()
                    .Where(g => g.DeletedAt == default)
                    .ToListAsync().ConfigureAwait(false);
                return groups;
        }

        public async Task SaveAsync(MerchantGroup entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.SaveAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateAsync(MerchantGroup entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task DeleteAsync(MerchantGroup entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.DeleteAsync(entity).ConfigureAwait(false);
        }
    }
}