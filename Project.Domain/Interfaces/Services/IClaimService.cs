using Project.Domain.DTOs;
using Project.Domain.DTOs.Requests;

namespace Project.Domain.Interfaces.Services
{
    public interface IClaimService
    {
        Task<(IEnumerable<ClaimDto> Items, int Total)> GetList(ClaimPagingRequestDto param);
        Task<ClaimDto> GetDetail(Guid Id);
        Task<Guid> Add(AddClaimRequestDto param);
        Task<Guid> ModifyStatus(ClaimModifyStatusRequestDto param);
        Task AutoApproveProcessingClaimsAsync();
    }

}
