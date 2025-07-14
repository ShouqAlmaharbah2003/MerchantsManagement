using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Enums;
using CommonLib.Interfaces;
using CommonLib.Utils;
using DataLib.Interfaces;
using DataLib.Models;
using NHibernate;
using StackExchange.Redis;
using Serilog;
using System.Text;

namespace CommonLib.Services
{
    public class MerchantsService : IMerchantsService
    {
        private readonly IMerchantsRepository _repository;
        private readonly IMerchantBranchesRepository _branchesRepository;
        private readonly ISession _session;
        private readonly IDatabase _cache;

        public MerchantsService(
            IMerchantsRepository repository,
            IMerchantBranchesRepository branchesRepository,
            ISession session,
            IConnectionMultiplexer redis)
        {
            _repository = repository;
            _branchesRepository = branchesRepository;
            _session = session;
            _cache = redis.GetDatabase();
        }

        public async Task CreateMerchantAsync(MerchantRequest dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Merchant request cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Name_Ar) || string.IsNullOrWhiteSpace(dto.Name_En))
                throw new ArgumentException("Merchant names cannot be empty.", nameof(dto));

            if (dto.MerchantGroupId <= 0)
                throw new ArgumentException("MerchantGroupId must be a positive integer.", nameof(dto.MerchantGroupId));

            var merchant = new Merchant
            {
                Name_Ar = dto.Name_Ar,
                Name_En = dto.Name_En,
                BusinessType = dto.BusinessType,
                MerchantGroup = new MerchantGroup { Id = dto.MerchantGroupId },
                ManagerName = dto.ManagerName,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
                var transaction = _session.BeginTransaction();

                await _repository.SaveAsync(merchant).ConfigureAwait(false);
                await transaction.CommitAsync();
        }

        public async Task ChangeMerchantGroupIdAsync(int merchantId, int newGroupId)
        {
            if (merchantId <= 0)
                throw new ArgumentException("MerchantId must be a positive integer.", nameof(merchantId));
            if (newGroupId <= 0)
                throw new ArgumentException("NewGroupId must be a positive integer.", nameof(newGroupId));

            var merchant = await _repository.GetByIdAsync(merchantId).ConfigureAwait(false);
            if (merchant == null)
            {
                Log.Warning("Merchant not found for MerchantId: {MerchantId}", merchantId);
                throw new Exception(LocalizationHelper.GetLocalizedString("NotFound", "en"));
            }

            merchant.MerchantGroup.Id = newGroupId;
            merchant.UpdatedAt = DateTime.UtcNow;

                var transaction = _session.BeginTransaction();

                await _repository.UpdateAsync(merchant).ConfigureAwait(false);
                await transaction.CommitAsync();
                Log.Information("Group ID changed successfully for merchant with Id: {MerchantId}", merchantId);

                await _cache.KeyDeleteAsync($"Merchant_{merchantId}").ConfigureAwait(false);
        }

        public async Task<string> GenerateSearchResultsHtmlAsync(string name, string mobile, int cityId, string branchName)
        {
            var merchants = await SearchMerchantsAsync(name, mobile, cityId, branchName).ConfigureAwait(false);

            var sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.Append("<h1>Search Results</h1>");
            sb.Append("<table border='1' style='border-collapse: collapse; width: 100%;'>");
            sb.Append("<tr>");
            sb.Append("<th>ID</th><th>Name (AR)</th><th>Name (EN)</th><th>Business Type</th><th>Manager</th><th>Status</th>");
            sb.Append("</tr>");

            foreach (var m in merchants)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{m.Id}</td>");
                sb.Append($"<td>{m.Name_Ar}</td>");
                sb.Append($"<td>{m.Name_En}</td>");
                sb.Append($"<td>{m.BusinessType}</td>");
                sb.Append($"<td>{m.ManagerName}</td>");
                sb.Append($"<td>{m.Status}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public async Task<string> GenerateSearchWithBranchesResultsHtmlAsync(string name, string mobile, int cityId, string branchName)
        {
            var merchants = await SearchMerchantsAsync(name, mobile, cityId, branchName).ConfigureAwait(false);

            var sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.Append("<h1>Search Results With Branches</h1>");

            foreach (var m in merchants)
            {
                sb.Append("<div style='margin-bottom: 20px;'>");
                sb.Append($"<h2>Merchant: {m.Name_Ar} / {m.Name_En}</h2>");

                var branches = (await _branchesRepository.GetBranchesByMerchantIdAsync(m.Id).ConfigureAwait(false)).ToList();
                if (branches.Count == 0)
                {
                    sb.Append("<p>No branches found.</p>");
                }
                else
                {
                    sb.Append("<table border='1' style='border-collapse: collapse; width: 100%;'>");
                    sb.Append("<tr>");
                    sb.Append("<th>Branch ID</th><th>Name (AR)</th><th>Name (EN)</th><th>Address</th><th>Status</th>");
                    sb.Append("</tr>");

                    foreach (var b in branches)
                    {
                        sb.Append("<tr>");
                        sb.Append($"<td>{b.Id}</td>");
                        sb.Append($"<td>{b.BranchName_Ar}</td>");
                        sb.Append($"<td>{b.BranchName_En}</td>");
                        sb.Append($"<td>{b.Address}</td>");
                        sb.Append($"<td>{(b.Status == 1 ? "Active" : "Inactive")}</td>");
                        sb.Append("</tr>");
                    }

                    sb.Append("</table>");
                }
                sb.Append("</div>");
            }

            sb.Append("</body></html>");
            return sb.ToString();
        }

        public async Task<List<MerchantResponse>> GetActiveMerchantsAsync()
        {
            string cacheKey = "ActiveMerchants";
            var cachedData = await _cache.StringGetAsync(cacheKey).ConfigureAwait(false);
            if (cachedData.HasValue)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<MerchantResponse>>(cachedData);
            }

                var merchants = await _repository.GetActiveMerchantsAsync().ConfigureAwait(false);
                var result = new List<MerchantResponse>();
                foreach (Merchant m in merchants)
                {
                    result.Add(new MerchantResponse
                    {
                        Id = m.Id,
                        Name_Ar = m.Name_Ar,
                        Name_En = m.Name_En,
                        BusinessType = m.BusinessType,
                        ManagerName = m.ManagerName,
                        Status = StatusEnum.Active.ToString()
                    });
                }
                Log.Information("Retrieved {Count} active merchants.", result.Count);
                await _cache.StringSetAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(result), TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                return result;
        }

        public async Task<MerchantResponse> GetMerchantDetailsByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive integer.", nameof(id));

            string cacheKey = $"Merchant_{id}";
            var cachedData = await _cache.StringGetAsync(cacheKey).ConfigureAwait(false);
            if (cachedData.HasValue)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<MerchantResponse>(cachedData);
            }

            var merchant = await _repository.GetByIdAsync(id).ConfigureAwait(false);
            if (merchant == null)
            {
                Log.Warning("Merchant not found for Id: {Id}", id);
                throw new Exception(LocalizationHelper.GetLocalizedString("NotFound", "en"));
            }

                var response = new MerchantResponse
                {
                    Id = merchant.Id,
                    Name_Ar = merchant.Name_Ar,
                    Name_En = merchant.Name_En,
                    BusinessType = merchant.BusinessType,
                    ManagerName = merchant.ManagerName,
                    Status = merchant.Status == 1 ? "Active" : "Inactive"
                };
                await _cache.StringSetAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(response), TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                return response;
        }

        public async Task<MerchantBranchResponse> GetMerchantMainBranchAsync(int merchantId)
        {
            if (merchantId <= 0)
                throw new ArgumentException("MerchantId must be a positive integer.", nameof(merchantId));

            var branch = await _branchesRepository.GetMainBranchByMerchantIdAsync(merchantId).ConfigureAwait(false) as MerchantBranch;
            if (branch == null)
            {
                Log.Warning("Main branch not found for MerchantId: {MerchantId}", merchantId);
                throw new Exception(LocalizationHelper.GetLocalizedString("NotFound", "en"));
            }

                return new MerchantBranchResponse
                {
                    Id = branch.Id,
                    BranchName_Ar = branch.BranchName_Ar,
                    BranchName_En = branch.BranchName_En,
                    Address = branch.Address,
                    Status = branch.Status == 1 ? "Active" : "Inactive"
                };
        }

        public async Task<List<MerchantResponse>> GetMerchantsByGroupIdAsync(int groupId)
        {
            if (groupId <= 0)
                throw new ArgumentException("GroupId must be a positive integer.", nameof(groupId));

                var merchants = await _repository.GetMerchantsByGroupIdAsync(groupId).ConfigureAwait(false);
                var result = new List<MerchantResponse>();
                foreach (Merchant m in merchants)
                {
                    result.Add(new MerchantResponse
                    {
                        Id = m.Id,
                        Name_Ar = m.Name_Ar,
                        Name_En = m.Name_En,
                        BusinessType = m.BusinessType,
                        ManagerName = m.ManagerName,
                        Status = m.Status == 1 ? "Active" : "Inactive"
                    });
                }
                Log.Information("Retrieved {Count} merchants for GroupId: {GroupId}", result.Count, groupId);
                return result;
        }

        public async Task<MerchantWithBranchesResponse> GetMerchantWithBranchesAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive integer.", nameof(id));

            var merchant = await _repository.GetByIdAsync(id).ConfigureAwait(false);
            if (merchant == null)
            {
                Log.Warning("Merchant not found for Id: {Id}", id);
                throw new Exception(LocalizationHelper.GetLocalizedString("NotFound", "en"));
            }

                var branches = (await _branchesRepository.GetBranchesByMerchantIdAsync(id).ConfigureAwait(false)).ToList();
                var response = new MerchantWithBranchesResponse
                {
                    MerchantId = merchant.Id,
                    MerchantName_Ar = merchant.Name_Ar,
                    MerchantName_En = merchant.Name_En,
                    Branches = new List<MerchantBranchResponse>()
                };

                foreach (MerchantBranch b in branches)
                {
                    response.Branches.Add(new MerchantBranchResponse
                    {
                        Id = b.Id,
                        BranchName_Ar = b.BranchName_Ar,
                        BranchName_En = b.BranchName_En,
                        Address = b.Address,
                        Status = b.Status == 1 ? "Active" : "Inactive"
                    });
                }
                return response;
        }

        public async Task<List<MerchantResponse>> SearchMerchantsAsync(string name, string mobile, int cityId, string branchName)
        {
                var query = (await _repository.GetAllAsync().ConfigureAwait(false)).AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(m =>
                        m.Name_Ar.Contains(name) || m.Name_En.Contains(name));
                }

                if (!string.IsNullOrEmpty(mobile))
                {
                    var branchMerchantIds = (await _branchesRepository.GetAllAsync().ConfigureAwait(false))
                        .Where(b => b.Mobile != null && b.Mobile.Contains(mobile))
                        .Select(b => b.Merchant.Id)
                        .Distinct()
                        .ToList();

                    query = query.Where(m => branchMerchantIds.Contains(m.Id));
                }

                if (cityId > 0)
                {
                    var branchMerchantIds = (await _branchesRepository.GetAllAsync().ConfigureAwait(false))
                        .Where(b => b.CityId == cityId)
                        .Select(b => b.Merchant.Id)
                        .Distinct()
                        .ToList();

                    query = query.Where(m => branchMerchantIds.Contains(m.Id));
                }

                if (!string.IsNullOrEmpty(branchName))
                {
                    var branchMerchantIds = (await _branchesRepository.GetAllAsync().ConfigureAwait(false))
                        .Where(b => b.BranchName_Ar.Contains(branchName) || b.BranchName_En.Contains(branchName))
                        .Select(b => b.Merchant.Id)
                        .Distinct()
                        .ToList();

                    query = query.Where(m => branchMerchantIds.Contains(m.Id));
                }

                var filteredMerchants = query.ToList();

                var result = new List<MerchantResponse>();
                foreach (var m in filteredMerchants)
                {
                    result.Add(new MerchantResponse
                    {
                        Id = m.Id,
                        Name_Ar = m.Name_Ar,
                        Name_En = m.Name_En,
                        BusinessType = m.BusinessType,
                        ManagerName = m.ManagerName,
                        Status = m.Status == 1 ? "Active" : "Inactive"
                    });
                }
                Log.Information("Found {Count} merchants.", result.Count);
                return result;
        }

        public async Task UpdateMerchantDetailsAsync(int merchantId, MerchantRequest dto)
        {
            if (merchantId <= 0)
                throw new ArgumentException("MerchantId must be a positive integer.", nameof(merchantId));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Merchant request cannot be null.");

            var merchant = await _repository.GetByIdAsync(merchantId).ConfigureAwait(false);
            if (merchant == null)
            {
                Log.Warning("Merchant not found for MerchantId: {MerchantId}", merchantId);
                throw new Exception(LocalizationHelper.GetLocalizedString("NotFound", "en"));
            }

            merchant.Name_Ar = dto.Name_Ar;
            merchant.Name_En = dto.Name_En;
            merchant.BusinessType = dto.BusinessType;
            merchant.ManagerName = dto.ManagerName;
            merchant.Status = dto.Status;
            merchant.UpdatedAt = DateTime.UtcNow;

                var transaction = _session.BeginTransaction();

                await _repository.UpdateAsync(merchant).ConfigureAwait(false);
                await transaction.CommitAsync();
                Log.Information("Merchant details updated successfully for Id: {MerchantId}", merchantId);
                await _cache.KeyDeleteAsync($"Merchant_{merchantId}").ConfigureAwait(false);
        }
    }
}