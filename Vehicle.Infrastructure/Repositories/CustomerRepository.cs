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
}

