using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Mappings
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Claims");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProviderCode).HasMaxLength(100).IsRequired();
            builder.Property(x => x.MemberId).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status).HasConversion<int>().IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasMany(c => c.Histories)
                .WithOne()
                .HasForeignKey(ch => ch.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata
                .FindNavigation(nameof(Claim.Histories))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
