using System;
using System.Collections.Generic;
using Project.Domain.Enums;

namespace Project.Domain.Entities
{
    public class Claim
    {
        public Guid Id { get; private set; }
        public string ProviderCode { get; private set; }
        public string MemberId { get; private set; }
        public decimal Amount { get; private set; }
        public ClaimStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }


        private readonly List<ClaimHistory> _histories = new();
        public IReadOnlyCollection<ClaimHistory> Histories => _histories.AsReadOnly();


        protected Claim() { }


        public Claim(Guid id, string providerCode, string memberId, decimal amount)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ProviderCode = providerCode ?? throw new ArgumentNullException(nameof(providerCode));
            MemberId = memberId ?? throw new ArgumentNullException(nameof(memberId));
            Amount = amount;
            Status = ClaimStatus.Submitted;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }


        public void ChangeStatus(ClaimStatus newStatus)
        {
            if (Status == newStatus) return;

            var valid = (Status == ClaimStatus.Submitted && newStatus == ClaimStatus.Processing) ||
            (Status == ClaimStatus.Processing && (newStatus == ClaimStatus.Approved || newStatus == ClaimStatus.Rejected));

            if (!valid) throw new InvalidOperationException($"Invalid status transition from {Status} to {newStatus}");

            var old = Status;
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;


            var history = new ClaimHistory(Guid.NewGuid(), Id, old, newStatus, DateTime.UtcNow);
            _histories.Add(history);
        }


        public void AddHistory(ClaimHistory history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            _histories.Add(history);
        }
    }
}
