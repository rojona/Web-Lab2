using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public interface IProductRep : IRep<Product>
{
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<Product> GetByProductNumberAsync(string productNumber);
    Task<IEnumerable<Product>> GetByStatusAsync(bool isDiscontinued);
}