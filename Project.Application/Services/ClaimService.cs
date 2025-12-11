using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Project.Domain.DTOs;
using Project.Domain.DTOs.Requests;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.Interfaces.Repositories;
using Project.Domain.Interfaces.Services;

namespace Project.Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _repo;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ClaimService> _logger;
        private readonly IValidator<AddClaimRequestDto> _addValidator;
        private readonly IValidator<ClaimModifyStatusRequestDto> _modifyValidator;

        public ClaimService(
            IClaimRepository repo,
            IMemoryCache cache,
            ILogger<ClaimService> logger,
            IValidator<AddClaimRequestDto> addValidator,
            IValidator<ClaimModifyStatusRequestDto> modifyValidator)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
            _addValidator = addValidator;
            _modifyValidator = modifyValidator;
        }

        public async Task<(IEnumerable<ClaimDto> Items, int Total)> GetList(ClaimPagingRequestDto request)
        {
            var (items, total) = await _repo.QueryAsync(request);

            var dtos = items.Select(c => new ClaimDto
            {
                Id = c.Id,
                ProviderCode = c.ProviderCode,
                MemberId = c.MemberId,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });


            return (dtos, total);
        }

        public async Task<ClaimDto> GetDetail(Guid Id)
        {
            var cacheKey = $"claim_detail_{Id}";
            if (_cache.TryGetValue(cacheKey, out ClaimDto? cached))
            {
                _logger.LogInformation("Cache hit for {Key}", cacheKey);
                return cached!;
            }

            var claim = await _repo.GetByIdAsync(Id);
            if (claim == null) throw new KeyNotFoundException("Claim not found");

            var dto = new ClaimDto
            {
                Id = claim.Id,
                ProviderCode = claim.ProviderCode,
                MemberId = claim.MemberId,
                Amount = claim.Amount,
                Status = claim.Status.ToString(),
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt,
                Histories = claim.Histories.Select(h => new ClaimHistoryDto
                {
                    Id = h.Id,
                    ClaimId = h.ClaimId,
                    OldStatus = h.OldStatus.ToString(),
                    NewStatus = h.NewStatus.ToString(),
                    ChangedAt = h.ChangedAt
                }).ToList()
            };


            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            _cache.Set(cacheKey, dto, cacheEntryOptions);


            return dto;
        }

        public async Task<Guid> Add(AddClaimRequestDto param)
        {
            try
            {
                var validation = await _addValidator.ValidateAsync(param);
                if (!validation.IsValid)
                    throw new ValidationException(validation.Errors);

                var claim = new Claim(Guid.NewGuid(), param.ProviderCode, param.MemberId, param.Amount);
                await _repo.AddAsync(claim);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Claim created: {ClaimId} for provider {Provider}", claim.Id, claim.ProviderCode);

                return claim.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding claim for provider {Provider}", param.ProviderCode);

                return Guid.Empty;
            }
        }

        public async Task<Guid> ModifyStatus(ClaimModifyStatusRequestDto param)
        {
            try
            {
                var validation = await _modifyValidator.ValidateAsync(param);
                if (!validation.IsValid)
                    throw new ValidationException(validation.Errors);

                var claim = await _repo.GetByIdAsync(param.ClaimId);
                if (claim == null) throw new KeyNotFoundException("Claim not found");

                if (!Enum.TryParse<ClaimStatus>(param.NewStatus, true, out var newStatus))
                    throw new ArgumentException("Invalid status value");

                claim.ChangeStatus(newStatus);

                await _repo.SaveChangesAsync();
                
                var dto = new ClaimDto
                {
                    Id = claim.Id,
                    ProviderCode = claim.ProviderCode,
                    MemberId = claim.MemberId,
                    Amount = claim.Amount,
                    Status = param.NewStatus,
                    CreatedAt = claim.CreatedAt,
                    UpdatedAt = claim.UpdatedAt,
                    Histories = claim.Histories.Select(h => new ClaimHistoryDto
                    {
                        Id = h.Id,
                        ClaimId = h.ClaimId,
                        OldStatus = h.OldStatus.ToString(),
                        NewStatus = h.NewStatus.ToString(),
                        ChangedAt = h.ChangedAt
                    }).ToList()
                };

                var cacheKey = $"claim_detail_{claim.Id}";
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(cacheKey, dto, cacheEntryOptions);

                _logger.LogInformation("Claim {ClaimId} status changed to {Status}", claim.Id, newStatus);

                return param.ClaimId;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error modifying claim {ClaimId}", param.ClaimId);

                return Guid.Empty;
            }
        }

        public async Task AutoApproveProcessingClaimsAsync()
        {
            var expiredClaims = await _repo.GetProcessingClaimsOlderThanAsync(TimeSpan.FromMinutes(5));

            foreach (var claim in expiredClaims)
            {
                claim.ChangeStatus(ClaimStatus.Approved);
            }

            await _repo.SaveChangesAsync();
        }

    }
}
