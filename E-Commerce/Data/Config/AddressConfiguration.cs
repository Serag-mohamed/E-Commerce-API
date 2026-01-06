using E_Commerce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Config
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.City) 
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Street)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.HomePositionDescription)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(a => a.User)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.UserId) 
                .IsRequired();
        }
    }
}
