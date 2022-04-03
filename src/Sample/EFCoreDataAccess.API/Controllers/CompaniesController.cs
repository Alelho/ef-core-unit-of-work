using EFCoreDataAccess.API.Requests;
using EFCoreDataAccess.Data;
using EFCoreDataAccess.Data.Repositories;
using EFCoreDataAccess.Models;
using EFCoreUnitOfWork.Builders;
using EFCoreUnitOfWork.Interfaces;
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
			// Should using the unit of work to get the generic repository
			var repository = _unitOfWork.GetGenericRepository<Company>();

			// Use 'IncludeQuery' to include child data in the query
			var includeQuery = IncludeQuery<Company>.Builder()
				.Include(c => c.Include(o => o.Address));

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

			// Multiples inclusions
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
			// Initiate a new transaction
			_unitOfWork.BeginTransaction(isolationLevel: System.Data.IsolationLevel.ReadCommitted);

			var companyRepository = _unitOfWork.GetGenericRepository<Company>();
			var addressRepository = _unitOfWork.GetRepository<AddressRepository>();

			var address = new Address(request.Street, request.City, request.State, request.Country, request.PostalCode);

			addressRepository.Add(address);

			// Keep the add operation above in memory during the transaction scope
			_unitOfWork.SaveChanges();

			var company = new Company(request.CompanyName);
			company.SetAddress(address.Id);

			companyRepository.Add(company);

			_unitOfWork.SaveChanges();

			try
			{
				// Commit all changes into the database
				_unitOfWork.Commit();
			}
			catch
			{
				// Rollback all changes
				_unitOfWork.Rollback();
			}

			return Ok();
		}
	}
}