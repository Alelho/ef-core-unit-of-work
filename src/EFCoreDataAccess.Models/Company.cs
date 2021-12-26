using EFCoreDataAccess.Models.Interface;
using System.Collections.Generic;

namespace EFCoreDataAccess.Models
{
    public class Company : IEntity
    {
        public Company(string name)
        {
            Name = name;
            Employees = new List<Employee>();
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public long AddressId { get; private set; }
        public Address Address { get; private set; }
        public ICollection<Employee> Employees { get; private set; }

        public void SetAddress(long addressId)
        {
            AddressId = addressId;
        }

        public void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
        }
    }
}
