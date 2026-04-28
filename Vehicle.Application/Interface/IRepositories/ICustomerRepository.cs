using Vehicle.Domain.Models;

namespace Vehicle.Application.Interface.IRepositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByUserIdAsync(string userId);
    Task<Customer> AddAsync(Customer customer);
}

