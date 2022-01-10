using EFCoreDataAccess.Models.Interface;
using System;

namespace EFCoreDataAccess.Models
{
    public class Employee : IEntity
    {
        public Employee(string name, string code, string position, DateTime birthDate)
        {
            Name = name;
            Code = code;
            Position = position;
            BirthDate = birthDate;
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Position { get; private set; }
        public DateTime BirthDate { get; private set; }
        public long CompanyId { get; private set; }
        public virtual Company Company { get; private set; }

        public void SetCompany(long companyId)
        {
            if (CompanyId > 0) throw new InvalidOperationException("company registered already");

            CompanyId = companyId;
        }
    }
}
