using E_Commerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.ParentCategoryId)
                .IsRequired(false);

            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
