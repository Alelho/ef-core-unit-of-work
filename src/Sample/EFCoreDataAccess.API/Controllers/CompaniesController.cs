using EFCoreDataAccess.API.Requests;
using EFCoreDataAccess.Builders;
using EFCoreDataAccess.Data;
using EFCoreDataAccess.Data.Repositories;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EFCoreDataAccess.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : Controller
    {
        private IUnitOfWork<EmployeeDbContext> _unitOfWork;

        public CompaniesController(
            IUnitOfWork<EmployeeDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(Company), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetCompany(long companyId)
        {
            var repository = _unitOfWork.GetGenericRepository<Company>();

            var includeQuery = IncludeQuery<Company>.Builder().Include(c => c.Include(o => o.Address));
            var company = repository.SingleOrDefault(f => f.Id == companyId, includeQuery);

            if (company == null) return NotFound();

            return Json(company);
        }

        [HttpGet("{companyId}/employees")]
        [ProducesResponseType(typeof(Company), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetCompanyWithEmployees(long companyId)
        {
            var repository = _unitOfWork.GetGenericRepository<Company>();

            var includeQuery = IncludeQuery<Company>.Builder()
                .Include(c => c.Include(o => o.Employees).ThenInclude(o => o.EmployeeEarnings))
                .Include(c => c.Include(o => o.Address));

            var company = repository.SingleOrDefault(f => f.Id == companyId, includeQuery);

            if (company == null) return NotFound();

            return Json(company);
        }

        [HttpPost("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CreateCompany([FromBody] CreateCompanyRequest request)
        {
            _unitOfWork.BeginTransaction();

            var companyRepository = _unitOfWork.GetGenericRepository<Company>();
            var addressRepository = _unitOfWork.GetRepository<AddressRepository>();

            var address = new Address(request.Street, request.City, request.State, request.Country, request.PostalCode);

            addressRepository.Add(address);

            _unitOfWork.SaveChanges();

            var company = new Company(request.CompanyName);
            company.SetAddress(address.Id);

            companyRepository.Add(company);

            _unitOfWork.SaveChanges();

            _unitOfWork.Commit();

            return Ok();
        }
    }
}