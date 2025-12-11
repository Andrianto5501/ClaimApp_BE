using Project.Domain.Enums;

namespace Project.Domain.DTOs.Requests
{
    public class ClaimPagingRequestDto
    {
        public ClaimStatus? Status { get; set; }
        public string? ProviderCode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public bool Descending { get; set; } = true;
    }

    public class AddClaimRequestDto
    {
        public string ProviderCode { get; set; } = "";
        public string MemberId { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class ClaimModifyStatusRequestDto
    {
        public Guid ClaimId { get; set; }
        public string NewStatus { get; set; } = "";
    }
}
