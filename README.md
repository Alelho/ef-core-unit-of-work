# ef-core-unit-of-work

[![build-and-tests](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml/badge.svg)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml)
[![build-and-publish](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml)
[![Coverage Status](https://coveralls.io/repos/github/Alelho/ef-core-unit-of-work/badge.svg)](https://coveralls.io/github/Alelho/ef-core-unit-of-work?branch=ef-core-unit-of-work-5)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This is an implementation of the following patterns, unit of work and generic repository for .NET Core. Unit of work provides a way to execute a bunch of operations (insert, update, delete and so on kinds) in a single transaction. The generic repository provides a set of basic operations like insert, find, update, etc for each database entity.

---

| Package | .NET Core | NuGet |
|---|---|---|
| EfCoreUnitOfWork | 5.x.x | ![Nuget](https://img.shields.io/nuget/v/EFCoreUnitOfWork) |

---

## Give a star! :star:

If you liked it or if this project helped you in any way, please give a star. Thanks!

## How to install
The package is available on the NuGet gallery. Run the command below to install in your project:

```
Install-Package EFCoreUnitOfWork -Version 5.0.0
```

## How to use
After package installation, register the DbContext into the DI container and call the extension method 'AddUnitOfWork' to register the unit of work.

````csharp
public void ConfigureServices(IServiceCollection services)
{
    var connectionString = @"Server=localhost;Database=EFCoreUnitOfWork;Uid=root;Pwd=123456;";

    // Register DbContext into DI container
    // It can use SQL Server, PostgreSQL instead of MySQL
    services.AddDbContext<EmployeeDbContext>(options =>
        options.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString))
        .LogTo(msg => Debug.WriteLine(msg), LogLevel.Error));

    // Add unit of work into DI container.
    // This is an exetensions from the EfCoreUnitOfWork package
    services.AddUnitOfWork<EmployeeDbContext>();
}
````

After the register and building of the DI container, the `UnitOfWork` can be injected into the constructors. In the below example, the `UnitOfWork` is injected into a controller.

````csharp
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
}
````






