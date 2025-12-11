using Project.Domain.DTOs.Requests;
using FluentAssertions;
using System.Net.Http.Json;

namespace Project.Test.Integration
{
    public class ClaimIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClaimIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Claim_EndToEnd_ShouldWork()
        {
            // 1. ADD CLAIM
            var addRequest = new AddClaimRequestDto
            {
                ProviderCode = "PT101",
                MemberId = "MT101",
                Amount = 150000
            };

            var addResponse = await _client.PostAsJsonAsync("/api/claims", addRequest);
            addResponse.EnsureSuccessStatusCode();

            var created = await addResponse.Content.ReadFromJsonAsync<CreateClaimResponse>();
            created.Should().NotBeNull();

            Guid claimId = created!.Id;
            claimId.Should().NotBe(Guid.Empty);

            // 2. GET DETAIL
            var detailResponse = await _client.GetAsync($"/api/claims/{claimId}");
            detailResponse.EnsureSuccessStatusCode();

            var detail = await detailResponse.Content.ReadAsStringAsync();
            detail.Should().Contain("PT101");
            detail.Should().Contain("MT101");

            // 3. MODIFY STATUS
            var modifyRequest = new ClaimModifyStatusRequestDto
            {
                ClaimId = claimId,
                NewStatus = "Processing"
            };

            var modifyResponse = await _client.PutAsJsonAsync($"/api/claims/{claimId}/status", modifyRequest);
            modifyResponse.EnsureSuccessStatusCode();

            var modified = await modifyResponse.Content.ReadFromJsonAsync<ModifyClaimResponse>();
            modified.Should().NotBeNull();
            modified!.Id.Should().Be(claimId);

            // 4. VERIFY UPDATED STATUS
            var checkResponse = await _client.GetAsync($"/api/claims/{claimId}");
            checkResponse.EnsureSuccessStatusCode();

            var detail2 = await checkResponse.Content.ReadAsStringAsync();
            detail2.Should().Contain("Processing");
        }
    }
    public record CreateClaimResponse(Guid Id);
    public record ModifyClaimResponse(Guid Id);
}
