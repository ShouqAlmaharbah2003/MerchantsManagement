using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Interfaces;
using DataLib.Interfaces;
using DataLib.Models;
using NHibernate;
using Serilog;

namespace CommonLib.Services
{
    public class MerchantBranchesService : IMerchantBranchesService
    {
        private readonly IMerchantBranchesRepository _repository;
        private readonly ISession _session;

        public MerchantBranchesService(IMerchantBranchesRepository repository, ISession session)
        {
            _repository = repository;
            _session = session;
        }

        public async Task CreateBranchAsync(MerchantBranchRequest dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Branch request cannot be null.");

            var merchant = await _session.GetAsync<Merchant>(dto.MerchantId).ConfigureAwait(false);
            if (merchant == null)
            {
                Log.Warning("Merchant not found for MerchantId: {MerchantId}", dto.MerchantId);
                throw new Exception("Merchant not found");
            }

            var user = await _session.GetAsync<User>(dto.ContactPersonId).ConfigureAwait(false);
            if (user == null)
            {
                Log.Warning("User not found for ContactPersonId: {ContactPersonId}", dto.ContactPersonId);
                throw new Exception("User not found");
            }

            var branch = new MerchantBranch
            {
                BranchName_Ar = dto.BranchName_Ar,
                BranchName_En = dto.BranchName_En,
                CityId = dto.CityId,
                GovernateId = dto.GovernateId,
                AlHat = dto.AlHat,
                Address = dto.Address,
                Region = dto.Region,
                Fax = dto.Fax,
                Website = dto.Website,
                Phone = dto.Phone,
                Mobile = dto.Mobile,
                Gps = dto.Gps,
                Status = dto.Status,
                MainBranch = dto.MainBranch,
                Merchant = merchant,
                ContactPersonId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

                var transaction = _session.BeginTransaction();
                await _repository.SaveAsync(branch).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
        }

        public async Task DeleteBranchAsync(int branchId)
        {
            if (branchId <= 0)
                throw new ArgumentException("BranchId must be a positive integer.", nameof(branchId));

            var branch = await _repository.GetByIdAsync(branchId).ConfigureAwait(false);
            if (branch == null)
            {
                Log.Warning("Branch not found for BranchId: {BranchId}", branchId);
                throw new Exception("Branch not found");
            }
                var transaction = _session.BeginTransaction();
                await _repository.DeleteAsync(branch).ConfigureAwait(false);
                await transaction.CommitAsync();
        }

        public async Task<List<MerchantBranchResponse>> GetAllBranchesAsync()
        {
                var branches = await _repository.GetAllAsync().ConfigureAwait(false);
                var result = new List<MerchantBranchResponse>();
                foreach (var branch in branches)
                {
                    result.Add(new MerchantBranchResponse
                    {
                        Id = branch.Id,
                        BranchName_Ar = branch.BranchName_Ar,
                        BranchName_En = branch.BranchName_En,
                        Address = branch.Address,
                        Status = branch.Status == 1 ? "Active" : "Inactive"
                    });
                }
                return result;
        }

        public async Task UpdateBranchAsync(int branchId, MerchantBranchRequest dto)
        {
            if (branchId <= 0)
                throw new ArgumentException("BranchId must be a positive integer.", nameof(branchId));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Branch request cannot be null.");

            var branch = await _repository.GetByIdAsync(branchId).ConfigureAwait(false);
            if (branch == null)
            {
                Log.Warning("Branch not found for BranchId: {BranchId}", branchId);
                throw new Exception("Branch not found");
            }

            branch.BranchName_Ar = dto.BranchName_Ar;
            branch.BranchName_En = dto.BranchName_En;
            branch.Status = dto.Status;
            branch.UpdatedAt = DateTime.UtcNow;

                var transaction = _session.BeginTransaction();
                await _repository.UpdateAsync(branch).ConfigureAwait(false);
                await transaction.CommitAsync();
        }
    }
}