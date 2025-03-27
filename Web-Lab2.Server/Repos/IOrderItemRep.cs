using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public interface IOrderItemRep : IRep<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(int orderId);
    Task<IEnumerable<OrderItem>> GetItemsByProductIdAsync(int productId);
}