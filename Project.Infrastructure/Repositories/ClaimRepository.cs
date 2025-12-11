using Project.Domain.Entities;
using Project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Interfaces.Repositories;
using Project.Domain.DTOs.Requests;
using Project.Domain.Enums;

namespace Project.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly AppDbContext _db;


        public ClaimRepository(AppDbContext db)
        {
            _db = db;
        }


        public async Task AddAsync(Claim claim)
        {
            await _db.Claims.AddAsync(claim);
        }


        public async Task<Claim?> GetByIdAsync(Guid id)
        {
            return await _db.Claims.Include(c => c.Histories).FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<(IEnumerable<Claim> Items, int Total)> QueryAsync(ClaimPagingRequestDto parameters)
        {
            var q = _db.Claims.AsQueryable();

            if (parameters.Status.HasValue)
                q = q.Where(x => x.Status == parameters.Status.Value);

            if (!string.IsNullOrEmpty(parameters.ProviderCode))
                q = q.Where(x => x.ProviderCode.Contains(parameters.ProviderCode));

            if (parameters.From.HasValue)
                q = q.Where(x => x.CreatedAt >= parameters.From.Value);

            if (parameters.To.HasValue)
                q = q.Where(x => x.CreatedAt <= parameters.To.Value);


            // sorting
            if (parameters.SortBy?.ToLower() == "createdat")
                q = parameters.Descending ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt);
            else if (parameters.SortBy?.ToLower() == "amount")
                q = parameters.Descending ? q.OrderByDescending(x => x.Amount) : q.OrderBy(x => x.Amount);
            else if (parameters.SortBy?.ToLower() == "status")
                q = parameters.Descending ? q.OrderByDescending(x => x.Status) : q.OrderBy(x => x.Status);
            else
                q = parameters.Descending ? q.OrderByDescending(x => x.UpdatedAt) : q.OrderBy(x => x.UpdatedAt);

            var total = await q.CountAsync();
            var items = await q.Skip((parameters.Page - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync();

            return (items, total);
        }


        public Task UpdateAsync(Claim claim)
        {
            _db.Claims.Update(claim);
            return Task.CompletedTask;
        }


        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Claim>> GetProcessingClaimsOlderThanAsync(TimeSpan duration)
        {
            var threshold = DateTime.UtcNow - duration;

            return await _db.Claims
                .Where(c => c.Status == ClaimStatus.Processing && c.UpdatedAt <= threshold)
                .ToListAsync();
        }

    }
}
