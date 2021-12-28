using EFCoreDataAccess.API.Requests;
using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Employee), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetEmployee(long id)
        {
            var employeeRepository = _unitOfWork.GetGenericRepository<Employee>();

            var employee = employeeRepository.SingleOrDefault(e => e.Id == id);

            if (employee == null) return NotFound();

            return Json(employee);
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetEmployeesByCompanyId(long companyId)
        {
            var employeeRepository = _unitOfWork.GetGenericRepository<Employee>();

            var employees = employeeRepository.Search(e => e.CompanyId == companyId);

            if (!employees.Any()) return NotFound();

            return Json(employees);
        }

        [HttpPost("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var employeeRepository = _unitOfWork.GetGenericRepository<Employee>();

            var employee = new Employee(request.Name, request.Code, request.Position, request.BirthDate);
            employee.SetCompany(request.CompanyId);

           employeeRepository.Add(employee);

            _unitOfWork.SaveChanges();

            return Ok();
        }
    }
}
