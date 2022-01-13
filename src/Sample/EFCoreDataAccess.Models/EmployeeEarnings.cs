using EFCoreDataAccess.Models.Enums;

namespace EFCoreDataAccess.Models
{
	public class EmployeeEarnings
	{
		public EmployeeEarnings(PaymentPeriodType paymentPeriodType)
		{
			PaymentPeriodType = paymentPeriodType;
		}

		public long Id { get; private set; }
		public decimal AnnualEarnings { get; private set; }
		public decimal MonthlyEarnings { get; private set; }
		public PaymentPeriodType PaymentPeriodType { get; private set; }
		public virtual Employee Employee { get; private set; }

		public void DefineAnnualEarnings(decimal annualEarnings)
		{
			AnnualEarnings = annualEarnings;
			MonthlyEarnings = annualEarnings / 12;
		}

		public void DefineMonthlyEarnings(decimal monthlyEarnings)
		{
			MonthlyEarnings = monthlyEarnings;
			AnnualEarnings = monthlyEarnings * 12;
		}
	}
}
