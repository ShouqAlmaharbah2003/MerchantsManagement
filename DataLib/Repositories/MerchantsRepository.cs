using DataLib.Models;
using DataLib.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace DataLib.Repositories
{
    public class MerchantsRepository : IMerchantsRepository
    {
        private readonly ISession _session;

        public MerchantsRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Merchant> GetByIdAsync(int id)
        {
                var merchant = await _session.GetAsync<Merchant>(id).ConfigureAwait(false);
                return merchant;
        }

        public async Task<IList<Merchant>> GetAllAsync()
        {
                var merchants = await _session.Query<Merchant>().ToListAsync().ConfigureAwait(false);
                return merchants;
        }

        public async Task<IList<Merchant>> GetActiveMerchantsAsync()
        {
                var merchants = await _session.Query<Merchant>()
                    .Where(m => m.Status == 1)
                    .ToListAsync().ConfigureAwait(false);
                return merchants;
        }

        public async Task<IList<Merchant>> GetByGroupIdAsync(int groupId)
        {
                var merchants = await _session.Query<Merchant>()
                    .Where(m => m.MerchantGroup.Id == groupId)
                    .ToListAsync().ConfigureAwait(false);
                return merchants;
        }

        public async Task<Merchant> GetMerchantWithBranchesAsync(int id)
        {
                var merchant = await _session.Query<Merchant>()
                    .Fetch(m => m.Branches)
                    .FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
                return merchant;
        }

        public async Task<MerchantBranch> GetMainBranchAsync(int merchantId)
        {
                var branch = await _session.Query<MerchantBranch>()
                    .FirstOrDefaultAsync(b => b.Merchant.Id == merchantId && b.MainBranch == "true" && b.DeletedAt == default).ConfigureAwait(false);
                return branch;
        }

        public async Task<IEnumerable<Merchant>> GetMerchantsByGroupIdAsync(int groupId)
        {
                var merchants = await _session.Query<Merchant>()
                    .Where(m => m.MerchantGroup.Id == groupId && m.DeletedAt == default)
                    .ToListAsync().ConfigureAwait(false);
                return merchants;
        }

        public async Task SaveAsync(Merchant entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.SaveAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Merchant entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Merchant entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.DeleteAsync(entity).ConfigureAwait(false);
        }
    }
}