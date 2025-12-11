using System;

namespace Project.Domain.Events
{
    public class ClaimStatusUpdatedEvent
    {
        public Guid ClaimId { get; }
        public string OldStatus { get; }
        public string NewStatus { get; }
        public DateTime ChangedAt { get; }


        public ClaimStatusUpdatedEvent(Guid claimId, string oldStatus, string newStatus, DateTime changedAt)
        {
            ClaimId = claimId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = changedAt;
        }
    }
}
