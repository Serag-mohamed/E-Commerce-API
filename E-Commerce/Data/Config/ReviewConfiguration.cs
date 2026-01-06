using E_Commerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => new { r.UserId, r.ProductId })
                .IsUnique();

            builder.Property(r => r.Rate)
                .IsRequired();

            builder.Property(r => r.Comment)
                .IsRequired();

            builder.Property(r => r.ReviewDate)
                .IsRequired();

            builder.HasOne(r => r.User)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.UserId)
                .IsRequired();

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .IsRequired();

        }
    }
}
