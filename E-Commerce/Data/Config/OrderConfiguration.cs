using E_Commerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalPrice)
                .HasPrecision(18,2)
                .IsRequired();

            builder.Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.HasOne(o => o.User)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired();

        }
    }
}
