using EFCoreDataAccess.Models;
using EFCoreDataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace EFCoreDataAccess.Data.Mappings
{
	public class EmployeeEarningsMap : IEntityTypeConfiguration<EmployeeEarnings>
	{
		public void Configure(EntityTypeBuilder<EmployeeEarnings> builder)
		{
			builder.ToTable("EmployeeEarnings");

			builder.Property(o => o.AnnualEarnings)
				.HasPrecision(10, 2);

			builder.Property(o => o.MonthlyEarnings)
				.HasPrecision(10, 2);

			builder.Property(o => o.PaymentPeriodType)
				.HasConversion(
					@enum => @enum.ToString(),
					value => (PaymentPeriodType)Enum.Parse(typeof(PaymentPeriodType), value))
				.HasMaxLength(32);
		}
	}
}
