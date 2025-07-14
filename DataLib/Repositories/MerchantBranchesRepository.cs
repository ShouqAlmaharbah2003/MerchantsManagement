using DataLib.Models;
using DataLib.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace DataLib.Repositories
{
    public class MerchantBranchesRepository : IMerchantBranchesRepository
    {
        private readonly ISession _session;

        public MerchantBranchesRepository(ISession session)
        {
            _session = session;
        }

        public async Task<MerchantBranch> GetByIdAsync(int id)
        {
                var branch = await _session.GetAsync<MerchantBranch>(id).ConfigureAwait(false);
                return branch;
        }

        public async Task<IList<MerchantBranch>> GetAllAsync()
        {
                var branches = await _session.Query<MerchantBranch>().ToListAsync().ConfigureAwait(false);
                return branches;
        }

        public async Task SaveAsync(MerchantBranch entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.SaveAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateAsync(MerchantBranch entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task DeleteAsync(MerchantBranch entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                    await _session.DeleteAsync(entity).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MerchantBranch>> GetBranchesByMerchantIdAsync(int merchantId)
        {
                var branches = await _session.Query<MerchantBranch>()
                    .Where(b => b.Merchant.Id == merchantId && b.DeletedAt == default)
                    .ToListAsync().ConfigureAwait(false);
                      return branches;
        }

        public async Task<MerchantBranch> GetMainBranchByMerchantIdAsync(int merchantId)
        {
                var branch = await _session.Query<MerchantBranch>()
                    .FirstOrDefaultAsync(b => b.Merchant.Id == merchantId && b.MainBranch == "true" && b.DeletedAt == default).ConfigureAwait(false);
                       return branch;
        }
    }
}