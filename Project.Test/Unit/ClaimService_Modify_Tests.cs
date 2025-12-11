using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Project.Application.Services;
using Project.Domain.DTOs.Requests;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.Interfaces.Repositories;


namespace Project.Test.Unit
{
    public class ClaimService_Modify_Tests
    {
        private readonly ClaimService _service;
        private readonly Mock<IClaimRepository> _repo = new();
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly Mock<ILogger<ClaimService>> _logger = new();
        private readonly Mock<IValidator<AddClaimRequestDto>> _addValidator = new();
        private readonly Mock<IValidator<ClaimModifyStatusRequestDto>> _modifyValidator = new();

        public ClaimService_Modify_Tests()
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
        public async Task Modify_ShouldUpdateStatus_WhenValid()
        {
            // arrange
            var claim = new Claim(ClaimTestState.CreatedClaimId, "PT001", "MT001", 1000);
            _repo.Setup(r => r.GetByIdAsync(claim.Id)).ReturnsAsync(claim);
            _repo.Setup(r => r.UpdateAsync(claim)).Returns(Task.CompletedTask);
            _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // act
            var result = await _service.ModifyStatus(new Domain.DTOs.Requests.ClaimModifyStatusRequestDto { ClaimId = claim.Id, NewStatus = "Processing" });

            // assert
            Assert.Equal(claim.Id, result);
            Assert.Equal(ClaimStatus.Processing, claim.Status);
        }
    }
}
