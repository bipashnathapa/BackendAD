using Microsoft.EntityFrameworkCore;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;

namespace Vehicle.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByUserIdAsync(string userId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.UserID == userId);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string search, int take = 20)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Array.Empty<Customer>();

        search = search.Trim();

        var query = _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .AsQueryable();

        if (int.TryParse(search, out var customerId))
        {
            query = query.Where(c => c.CustomerID == customerId);
        }
        else
        {
            var like = $"%{search}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.User.FullName, like) ||
                EF.Functions.ILike(c.User.Email!, like) ||
                (c.User.PhoneNumber != null && EF.Functions.ILike(c.User.PhoneNumber, like)) ||
                c.Vehicles.Any(v => EF.Functions.ILike(v.VehicleNo, like)));
        }

        return await query
            .OrderByDescending(c => c.CustomerID)
            .Take(take)
            .ToListAsync();
    }
}
