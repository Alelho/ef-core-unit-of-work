using EFCoreDataAccess.Data;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EFCoreDataAccess.Tests.Infra
{
    public static class SeedHelper
    {
        public static void Populate(IServiceProvider serviceProvider)
        {
            var employeeContext = serviceProvider.GetService<EmployeeDbContext>();

            employeeContext.Database.Migrate();

            var santaBarbaraAddress = new Address(
                street: "10, Little Finger",
                city: "Santa Barbara",
                state: "CA",
                country: "EUA",
                postalCode: "222333");

            var miamiAddress = new Address(
                street: "16, Lincoln",
                city: "Miami",
                state: "FL",
                country: "EUA",
                postalCode: "333456");

            var bostonAddress = new Address(
                street: "1, Palmer Square",
                city: "Boston",
                state: "MA",
                country: "EUA",
                postalCode: "123444");

            employeeContext.AddRange(new[]
            {
                santaBarbaraAddress,
                miamiAddress,
                bostonAddress
            });

            employeeContext.SaveChanges();

            var company1 = new Company(name: "Santa Barbara Police");
            var company2 = new Company(name: "Miame Metro Police");
            var company3 = new Company(name: "FBI");

            company1.SetAddress(santaBarbaraAddress.Id);

            company2.SetAddress(miamiAddress.Id);

            company3.SetAddress(bostonAddress.Id);

            var employee1 = new Employee(
                    name: "Shawn Spencer",
                    code: "1111",
                    position: "psychic detective",
                    birthDate: DateTime.UtcNow.AddYears(-29));

            var employeeEarnings1 = new EmployeeEarnings(PaymentPeriodType.Annual);
            employeeEarnings1.DefineAnnualEarnings(80_000);
            employee1.DefineEarnings(employeeEarnings1);

            var employee2 = new Employee(
                    name: "Dexer Morgan",
                    code: "0333",
                    position: "Blood Spatter Analyst",
                    birthDate: DateTime.UtcNow.AddYears(-35));

            var employeeEarnings2 = new EmployeeEarnings(PaymentPeriodType.Monthly);
            employeeEarnings2.DefineMonthlyEarnings(7_000);
            employee2.DefineEarnings(employeeEarnings2);

            var employee3 = new Employee(
                    name: "Olivia Dunham",
                    code: "0001",
                    position: "FBI Agent III",
                    birthDate: DateTime.UtcNow.AddYears(-32));

            var employeeEarnings3 = new EmployeeEarnings(PaymentPeriodType.Monthly);
            employeeEarnings3.DefineMonthlyEarnings(12_000);
            employee3.DefineEarnings(employeeEarnings3);

            company1.AddEmployee(employee1);
            company2.AddEmployee(employee2);
            company3.AddEmployee(employee3);

            employeeContext.AddRange(new[]
            {
                company1,
                company2,
                company3
            });

            employeeContext.SaveChanges();
        }
    }
}
