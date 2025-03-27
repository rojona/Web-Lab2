using Microsoft.EntityFrameworkCore;
using Web_Lab2.Data;
using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public class OrderItemRep : IOrderItemRep
{
    private readonly AppDbContext _context;

    public OrderItemRep(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        return await _context.OrderItems
            .Include(oi => oi.Product)
            .ToListAsync();
    }

    public async Task<OrderItem> GetByIdAsync(int id)
    {
        return await _context.OrderItems
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == id);
    }

    public async Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(int orderId)
    {
        return await _context.OrderItems
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<OrderItem>> GetItemsByProductIdAsync(int productId)
    {
        return await _context.OrderItems
            .Where(oi => oi.ProductId == productId)
            .ToListAsync();
    }

    public async Task<OrderItem> AddAsync(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);
        return orderItem;
    }

    public async Task UpdateAsync(OrderItem orderItem)
    {
        _context.Entry(orderItem).State = EntityState.Modified;
    }

    public async Task DeleteAsync(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);
        if (orderItem != null)
        {
            _context.OrderItems.Remove(orderItem);
        }
    }
}