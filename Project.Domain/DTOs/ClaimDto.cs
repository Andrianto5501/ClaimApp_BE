using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.DTOs
{
    public class ClaimDto
    {
        public Guid Id { get; set; }
        public string ProviderCode { get; set; }
        public string MemberId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ClaimHistoryDto> Histories { get; set; } = new();
    }
}
