using EFCoreDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreDataAccess.Data.Mappings
{
    public class EmployeeMap : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(o => o.Code)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.Property(o => o.Position)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(o => o.BirthDate)
                   .IsRequired();

            builder.HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId);

            builder.HasOne(e => e.EmployeeEarnings)
               .WithOne(o => o.Employee)
               .HasForeignKey<Employee>(e => e.EmployeeEarningsId);
        }
    }
}
