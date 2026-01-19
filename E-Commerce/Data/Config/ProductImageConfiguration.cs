using E_Commerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageUrl)
                .IsRequired();

            builder.Property(pi => pi.IsMain)
                .IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
