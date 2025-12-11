using System;
using Project.Domain.Enums;

namespace Project.Domain.Entities
{
    public class ClaimHistory
    {
        public Guid Id { get; private set; }
        public Guid ClaimId { get; private set; }
        public ClaimStatus OldStatus { get; private set; }
        public ClaimStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }


        protected ClaimHistory() { }


        public ClaimHistory(Guid id, Guid claimId, ClaimStatus oldStatus, ClaimStatus newStatus, DateTime changedAt)
        {
            Id = id;
            ClaimId = claimId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = changedAt;
        }
    }
}
