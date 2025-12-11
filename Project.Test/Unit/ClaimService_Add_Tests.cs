using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Project.Application.Services;
using Project.Domain.DTOs.Requests;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Repositories;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Project.Test.Unit
{
    public class ClaimService_Add_Tests
    {
        private readonly ClaimService _service;
        private readonly Mock<IClaimRepository> _repo = new();
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly Mock<ILogger<ClaimService>> _logger = new();
        private readonly Mock<IValidator<AddClaimRequestDto>> _addValidator = new();
        private readonly Mock<IValidator<ClaimModifyStatusRequestDto>> _modifyValidator = new();

        public ClaimService_Add_Tests()
        {
            _addValidator.Setup(v => v.ValidateAsync(It.IsAny<AddClaimRequestDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _modifyValidator.Setup(v => v.ValidateAsync(It.IsAny<ClaimModifyStatusRequestDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _service = new ClaimService(
                _repo.Object,
                _cache,
                _logger.Object,
                _addValidator.Object,
                _modifyValidator.Object
            );
        }

        [Fact]
        public async Task Add_ShouldReturnGuid_WhenSuccess()
        {
            Claim? savedClaim = null;

            _repo.Setup(r => r.AddAsync(It.IsAny<Claim>()))
                .Callback<Claim>(c => savedClaim = c)
                .Returns(Task.CompletedTask);

            _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var id = await _service.Add(new AddClaimRequestDto
            {
                ProviderCode = "PT001",
                MemberId = "MT001",
                Amount = 2500000
            });

            Assert.NotEqual(Guid.Empty, id);
            Assert.NotNull(savedClaim);
            Assert.Equal(savedClaim!.Id, id);

            ClaimTestState.CreatedClaimId = id;
        }
    }
}
