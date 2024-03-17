using EFCoreDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreDataAccess.Data.Mappings
{
    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            
            builder.HasKey(o => o.Id);

            builder.Property(o => o.State)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(o => o.City)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(o => o.Street)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(o => o.PostalCode)
                   .IsRequired()
                   .HasMaxLength(16);

            builder.Property(o => o.Country)
                   .IsRequired()
                   .HasMaxLength(32);
        }
    }
}
