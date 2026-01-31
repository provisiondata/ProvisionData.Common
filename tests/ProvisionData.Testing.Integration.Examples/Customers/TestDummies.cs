// ProvisionData.Common
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riok.Mapperly.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionData.Testing.Integration.Examples;

public class Customer
{
    // For EF Core
    protected Customer()
    {
    }

    public Customer(Guid id, String name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; internal set; }
    public String Name { get; internal set; } = String.Empty;
    public String? Email { get; internal set; } = String.Empty;

    public Customer SetName(String name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        return this;
    }

    public Customer SetEmail(String email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Email = email.Trim();
        return this;
    }
}

[Mapper]
public static partial class CustomerMapper
{
    public static partial CustomerDto ToDto(this Customer customer);
    public static partial Customer ToEntity(this CreateCustomerCommand dto);
}

public record CustomerDto(
    Guid Id,
    String Name,
    String? Email
);

public record CreateCustomerCommand(
    Guid Id,
    String Name,
    String? Email
);

public record GetCustomerByIdQuery(Guid Id);

public record UpdateCustomerCommand(
    Guid Id,
    String Name,
    String? Email
);

public interface ICustomerApplicationService
{
    Task<Result<Guid>> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken);
    Task<Result<CustomerDto>> GetCustomerByIdAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken);
    Task<Result> UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken);
}

public class CustomerApplicationService(ICustomerRepository repository, ILogger<CustomerApplicationService> logger) : ICustomerApplicationService
{
    private readonly ICustomerRepository _repository = repository;
    private readonly ILogger<CustomerApplicationService> _logger = logger;

    public async Task<Result<Guid>> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var entity = command.ToEntity();

        _logger.LogInformation("Creating customer with Id '{CustomerId}'", entity.Id);
        var result = await _repository.CreateAsync(entity, cancellationToken);
        _logger.LogInformation("Created customer with Id '{CustomerId}'", entity.Id);

        return result;
    }

    public async Task<Result<CustomerDto>> GetCustomerByIdAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(query.Id, cancellationToken);

        var dto = CustomerMapper.ToDto(customer);

        return Result<CustomerDto>.Success(dto);
    }

    public async Task<Result> UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (existing is null)
        {
            return Error.NotFound("Customer.NotFound", $"Customer with Id '{command.Id}' was not found.");
        }

        if (command.Name is not null)
        {
            existing.SetName(command.Name);
        }

        if (command.Email is not null)
        {
            existing.SetEmail(command.Email);
        }

        await _repository.UpdateAsync(existing, cancellationToken);

        return Result.Success();
    }
}

public interface ICustomerRepository
{
    Task<Result<Guid>> CreateAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Customer customer, CancellationToken cancellationToken);
}

public class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
    private readonly CustomerDbContext _dbContext = dbContext;

    public async Task<Result<Guid>> CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Customers
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity!;
    }

    public async Task<Result> UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        //var existing = await _dbContext.Customers
        //    .SingleOrDefaultAsync(e => e.Id == customer.Id, cancellationToken);

        //if (existing is not null)
        //{
        //    existing.Name = customer.Name;
        //    existing.Email = customer.Email;
        //}

        _dbContext.Customers.Update(customer);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

[SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
public class CustomerDbContext(DbContextOptions<CustomerDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers { get; init; } = default!;
}
