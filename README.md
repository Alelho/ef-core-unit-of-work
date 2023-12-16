# ef-core-unit-of-work

[![build-and-tests](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml/badge.svg?branch=ef-core-net-6)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml?branch=ef-core-net-6)
[![build-and-publish](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml/badge.svg?branch=ef-core-net-6)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml?branch=ef-core-net-6)
[![Coverage Status](https://coveralls.io/repos/github/Alelho/ef-core-unit-of-work/badge.svg)](https://coveralls.io/github/Alelho/ef-core-unit-of-work?branch=ef-core-net-6)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This is an implementation of the following patterns, unit of work and generic repository for .NET Core. Unit of work provides a way to execute a bunch of operations (insert, update, delete and so on kinds) in a single transaction. The generic repository provides a set of basic operations like insert, find, update, etc for each database entity.

---

| Package | .NET Core | NuGet |
|---|---|---|
| EfCoreUnitOfWork | 6.x.x | ![Nuget](https://img.shields.io/nuget/v/EFCoreUnitOfWork) |

---

## Give a star! :star:

If you liked it or if this project helped you in any way, please give a star. Thanks!

## How to install
The package is available on the NuGet gallery. Run the command below to install in your project:

```
Install-Package EFCoreUnitOfWork -Version 6.0.0
```

## How to use
After the package installation, register the DbContext into the DI container and call the extension method 'AddUnitOfWork' to register the unit of work.

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

Use the unit of work instance to get the repositories. The package provides a generic repository with some methods implemented.
````csharp
var repository = _unitOfWork.GetGenericRepository<Company>();
````

The below code get a custom repository
````csharp
public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(DbContext dbContext)
        : base(dbContext)
    { }
}
````
````csharp
var addressRepository = _unitOfWork.GetRepository<AddressRepository>();
````
> A custom repository can be created, inheriting or not from the 'Generic Repository', but the custom repository must have a constructor that injects only a DbContext. For the next version, the package will support creating a custom repository with N dependencies.

### List of operations
````csharp
T Add(T entity);
void AddRange(IEnumerable<T> entities);
void Update(T entity, params Expression<Func<T, object>>[] properties);
void UpdateRange(IEnumerable<T> entities);
void RemoveByEntity(T entity);
void RemoveSingle(Expression<Func<T, bool>> predicate);
void RemoveRange(IEnumerable<T> entities);

T SingleOrDefault(Expression<Func<T, bool>> predicate);
T SingleOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
T FirstOrDefault(Expression<Func<T, bool>> predicate);
T FirstOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
T LastOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> keySelector);
T LastOrDefault(
    Expression<Func<T, bool>> predicate,
    Expression<Func<T, object>> keySelector,
    IncludeQuery<T> includeQuery);
IEnumerable<T> Search(Expression<Func<T, bool>> predicate);
IEnumerable<T> Search(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
bool Any(Expression<Func<T, bool>> predicate = null);
long Count(Expression<Func<T, bool>> predicate = null);
````
> Most of these operations have an async version

### Unit of Work
The following example demonstrates how to start a transaction and run several operations and in the end commit all operations together into the database. If anything got wrong, all operations are undone through the Rollback method.

````csharp
[HttpPost("")]
[ProducesResponseType((int)HttpStatusCode.OK)]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
public IActionResult CreateCompany([FromBody] CreateCompanyRequest request)
{
    // Initiate a new transaction
    _unitOfWork.BeginTransaction();

    var companyRepository = _unitOfWork.GetGenericRepository<Company>();
    var addressRepository = _unitOfWork.GetRepository<AddressRepository>();

    var address = new Address(request.Street, request.City, request.State, request.Country, request.PostalCode);

    // Keep the add operation in memory during the transaction scope
    addressRepository.Add(address);

    var company = new Company(request.CompanyName);
    company.SetAddress(address.Id);

     // Keep this operation in memory during the transaction scope
    companyRepository.Add(company);

    // Commit all changes into the database
    _unitOfWork.Commit();

    return Ok();
}
````

The `UnitOfWork` has the following operations:
````csharp
void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
void Rollback();
void Commit();
int SaveChanges(bool acceptAllChangesOnSuccess = true);
````
> Those operations have an async version
