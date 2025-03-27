using Microsoft.EntityFrameworkCore;
using Web_Lab2.Data;
using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public class ProductRep : IProductRep
{
    private readonly AppDbContext _context;

    public ProductRep(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
    
    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Product> GetByProductNumberAsync(string productNumber)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.ProductNumber == productNumber);
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(bool isDiscontinued)
    {
        return await _context.Products
            .Where(p => p.IsDiscontinued == isDiscontinued)
            .ToListAsync();
    }
}