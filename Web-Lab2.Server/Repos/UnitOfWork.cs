using Web_Lab2.Data;

namespace Web_Lab2.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new ProductRep(_context);
        Customers = new CustomerRep(_context);
        Orders = new OrderRep(_context);
        OrderItems = new OrderItemRep(_context);
    }
    
    public IProductRep Products { get; set; }
    public ICustomerRep Customers { get; set; }
    public IOrderRep Orders { get; set; }
    public IOrderItemRep OrderItems { get; set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}