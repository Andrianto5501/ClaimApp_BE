using Project.Domain.Entities;
using Project.Domain.DTOs.Requests;

namespace Project.Domain.Interfaces.Repositories
{
    public interface IClaimRepository
    {
        Task AddAsync(Claim claim);
        Task<Claim?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Claim> Items, int Total)> QueryAsync(ClaimPagingRequestDto parameters);
        Task UpdateAsync(Claim claim);
        Task SaveChangesAsync();
        Task<IEnumerable<Claim>> GetProcessingClaimsOlderThanAsync(TimeSpan duration);
    }
}
