using E_Commerce.Entities;
using E_Commerce.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.paymentStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.PaidAt)
                .IsRequired();

            builder.HasOne(p => p.Order)
              .WithOne(o => o.Payment)
              .HasForeignKey<Payment>(p => p.OrderId)
              .IsRequired(false);
        }
    }
}
