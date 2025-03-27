using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public interface ICustomerRep : IRep<Customer>
{
    Task<Customer> GetByEmailAsync(string email);
}