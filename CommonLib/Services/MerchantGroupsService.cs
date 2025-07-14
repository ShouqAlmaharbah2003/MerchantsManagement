using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Interfaces;
using DataLib.Interfaces;
using DataLib.Models;
using NHibernate;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace CommonLib.Services
{
    public class MerchantGroupsService : IMerchantGroupsService
    {
        private readonly IMerchantGroupsRepository _repository;
        private readonly ISession _session;
        private readonly ILogger<MerchantGroupsService> _logger;
        private readonly IStringLocalizer<MerchantGroupsService> _localizer;
        private readonly DataLib.Caching.RedisCacheService _cacheService;

        public MerchantGroupsService(IMerchantGroupsRepository repository, ISession session, ILogger<MerchantGroupsService> logger, IStringLocalizer<MerchantGroupsService> localizer, DataLib.Caching.RedisCacheService cacheService)
        {
            _repository = repository;
            _session = session;
            _logger = logger;
            _localizer = localizer;
            _cacheService = cacheService;
        }

        public async Task CreateGroupAsync(MerchantGroupRequest dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), _localizer["GroupRequestCannotBeNull"]);

            var group = new MerchantGroup
            {
                Name_Ar = dto.Name_Ar,
                Name_En = dto.Name_En,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var transaction = _session.BeginTransaction();

                try
                {
                    if (!_session.IsOpen)
                    {
                        _logger.LogWarning("ISession is closed, reopening a new session.");
                        throw new InvalidOperationException(_localizer["SessionClosedError"]);
                    }

                    _logger.LogInformation(_localizer["GroupCreated"], dto.Name_Ar);
                    await _repository.SaveAsync(group).ConfigureAwait(false);
                    await transaction.CommitAsync();
                    _logger.LogInformation(_localizer["GroupCreatedSuccessfully"], group.Id);

                    await _cacheService.RemoveAsync("AllGroups").ConfigureAwait(false);
                    await _cacheService.RemoveAsync("ActiveGroups").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (transaction != null && transaction.IsActive)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                }
                    _logger.LogError(ex, _localizer["FailedToCreateGroup"], dto.Name_Ar, ex.Message, ex.StackTrace);
                    throw;
                }
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            if (groupId <= 0)
                throw new ArgumentException(_localizer["GroupIdMustBePositive"], nameof(groupId));

            var group = await _repository.GetByIdAsync(groupId).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning(_localizer["GroupNotFound"], groupId);
                throw new Exception(_localizer["GroupNotFound"]);
            }

            var transaction = _session.BeginTransaction();
      
                try
                {
                    if (!_session.IsOpen)
                    {
                        _logger.LogWarning("ISession is closed, reopening a new session.");
                        throw new InvalidOperationException(_localizer["SessionClosedError"]);
                    }

                    _logger.LogInformation(_localizer["GroupDeleted"], groupId);
                    await _repository.DeleteAsync(group).ConfigureAwait(false);
                    await transaction.CommitAsync();
                    _logger.LogInformation(_localizer["GroupDeletedSuccessfully"], groupId);

                    await _cacheService.RemoveAsync("AllGroups").ConfigureAwait(false);
                    await _cacheService.RemoveAsync("ActiveGroups").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (transaction != null && transaction.IsActive)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                }
                    _logger.LogError(ex, _localizer["FailedToDeleteGroup"], groupId, ex.Message, ex.StackTrace);
                    throw;
                }
        }

        public async Task<List<MerchantGroupResponse>> GetActiveGroupsAsync()
        {
            try
            {
                _logger.LogInformation(_localizer["RetrievingActiveGroups"]);
                var cacheKey = "ActiveGroups";
                var cachedGroups = await _cacheService.GetAsync<List<MerchantGroupResponse>>(cacheKey).ConfigureAwait(false);
                if (cachedGroups != null)
                {
                    _logger.LogInformation(_localizer["CacheRetrievedSuccessfully"], cacheKey);
                    return cachedGroups;
                }

                var groups = await _repository.GetActiveGroupsAsync().ConfigureAwait(false);
                var result = new List<MerchantGroupResponse>();
                foreach (var group in groups)
                {
                    result.Add(new MerchantGroupResponse
                    {
                        Id = group.Id,
                        Name_Ar = group.Name_Ar,
                        Name_En = group.Name_En,
                        Status = "Active"
                    });
                }
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
                _logger.LogInformation(_localizer["RetrievedActiveGroups"], result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["FailedToRetrieveActiveGroups"], ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<List<MerchantGroupResponse>> GetAllGroupsAsync()
        {
            try
            {
                _logger.LogInformation(_localizer["RetrievingAllGroups"]);
                var cacheKey = "AllGroups";
                var cachedGroups = await _cacheService.GetAsync<List<MerchantGroupResponse>>(cacheKey).ConfigureAwait(false);
                if (cachedGroups != null)
                {
                    _logger.LogInformation(_localizer["CacheRetrievedSuccessfully"], cacheKey);
                    return cachedGroups;
                }

                var groups = await _repository.GetAllAsync().ConfigureAwait(false);
                var result = new List<MerchantGroupResponse>();
                foreach (var group in groups)
                {
                    result.Add(new MerchantGroupResponse
                    {
                        Id = group.Id,
                        Name_Ar = group.Name_Ar,
                        Name_En = group.Name_En,
                        Status = group.DeletedAt == default ? "Active" : "Inactive"
                    });
                }
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
                _logger.LogInformation(_localizer["RetrievedAllGroups"], result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["FailedToRetrieveAllGroups"], ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task UpdateGroupName_ArAsync(int groupId, string newName)
        {
            if (groupId <= 0)
                throw new ArgumentException(_localizer["GroupIdMustBePositive"], nameof(groupId));
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException(_localizer["NewNameCannotBeNull"], nameof(newName));

            var group = await _repository.GetByIdAsync(groupId).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning(_localizer["GroupNotFound"], groupId);
                throw new Exception(_localizer["GroupNotFound"]);
            }

            group.Name_Ar = newName;
            group.UpdatedAt = DateTime.UtcNow;

            var transaction = _session.BeginTransaction();
     
                try
                {
                    if (!_session.IsOpen)
                    {
                        _logger.LogWarning("ISession is closed, reopening a new session.");
                        throw new InvalidOperationException(_localizer["SessionClosedError"]);
                    }

                    _logger.LogInformation(_localizer["GroupNameUpdated"], groupId);
                    await _repository.UpdateAsync(group).ConfigureAwait(false);
                    await transaction.CommitAsync();
                    _logger.LogInformation(_localizer["GroupNameUpdated"], groupId);

                    await _cacheService.RemoveAsync("AllGroups").ConfigureAwait(false);
                    await _cacheService.RemoveAsync("ActiveGroups").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (transaction != null && transaction.IsActive)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                }
                    _logger.LogError(ex, _localizer["FailedToUpdateGroupName"], groupId, ex.Message, ex.StackTrace);
                    throw;
                }
        }

        public async Task UpdateGroupName_EnAsync(int groupId, string newName)
        {
            if (groupId <= 0)
                throw new ArgumentException(_localizer["GroupIdMustBePositive"], nameof(groupId));
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException(_localizer["NewNameCannotBeNull"], nameof(newName));

            var group = await _repository.GetByIdAsync(groupId).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning(_localizer["GroupNotFound"], groupId);
                throw new Exception(_localizer["GroupNotFound"]);
            }

            group.Name_En = newName;
            group.UpdatedAt = DateTime.UtcNow;

            var transaction = _session.BeginTransaction();

                try
                {
                    if (!_session.IsOpen)
                    {
                        _logger.LogWarning("ISession is closed, reopening a new session.");
                        throw new InvalidOperationException(_localizer["SessionClosedError"]);
                    }

                    _logger.LogInformation(_localizer["GroupNameUpdated"], groupId);
                    await _repository.UpdateAsync(group).ConfigureAwait(false);
                    await transaction.CommitAsync();
                    _logger.LogInformation(_localizer["GroupNameUpdated"], groupId);
 
                    await _cacheService.RemoveAsync("AllGroups").ConfigureAwait(false);
                    await _cacheService.RemoveAsync("ActiveGroups").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (transaction != null && transaction.IsActive)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                }
                    _logger.LogError(ex, _localizer["FailedToUpdateGroupName"], groupId, ex.Message, ex.StackTrace);
                    throw;
                }
        }
    }
}