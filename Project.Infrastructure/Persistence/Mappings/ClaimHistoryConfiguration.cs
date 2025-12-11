using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Mappings
{
    public class ClaimHistoryConfiguration : IEntityTypeConfiguration<ClaimHistory>
    {
        public void Configure(EntityTypeBuilder<ClaimHistory> builder)
        {
            builder.ToTable("ClaimHistories");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.ClaimId).IsRequired();
            builder.Property(x => x.OldStatus).HasConversion<int>().IsRequired();
            builder.Property(x => x.NewStatus).HasConversion<int>().IsRequired();
            builder.Property(x => x.ChangedAt).IsRequired();
        }
    }
}
