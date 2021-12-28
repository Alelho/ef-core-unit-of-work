using System;

namespace EFCoreDataAccess.API.Requests
{
    public class CreateEmployeeRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Position { get; set; }
        public DateTime BirthDate { get; set; }
        public long CompanyId { get; set; }
    }
}
