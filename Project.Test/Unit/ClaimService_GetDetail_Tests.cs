using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Project.Application.Services;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Repositories;

namespace Project.Test.Unit
{
    public class ClaimService_GetDetail_Tests
    {
        private readonly ClaimService _service;
        private readonly Mock<IClaimRepository> _repo = new();
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private readonly Mock<ILogger<ClaimService>> _logger = new();

        public ClaimService_GetDetail_Tests()
        {
            _service = new ClaimService(_repo.Object, _cache, _logger.Object, null, null);
        }

        [Fact]
        public async Task GetDetail_ShouldUseCache_OnSecondCall()
        {
            // arrange
            var id = ClaimTestState.CreatedClaimId;
            var claim = new Claim(id, "PT001", "MT001", 1000);
            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(claim);

            // act
            await _service.GetDetail(id); // repo hit
            await _service.GetDetail(id); // cache hit

            // assert
            _repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        }
    }
}
