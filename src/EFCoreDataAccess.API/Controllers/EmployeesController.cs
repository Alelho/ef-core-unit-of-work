using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EFCoreDataAccess.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : Controller
    {
        private IUnitOfWork<EmployeeDbContext> _unitOfWork;

        public EmployeesController(IUnitOfWork<EmployeeDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("company/{companyId}")]
        public IActionResult GetEmployeesByCompanyId(long companyId)
        {
            var employeeRepository = _unitOfWork.GetGenericRepository<Employee>();

            var employees = employeeRepository.Search(e => e.CompanyId == companyId);

            if (!employees.Any()) return NotFound();

            return Json(employees);
        }
    }
}
