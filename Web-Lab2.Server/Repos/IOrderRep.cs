using Web_Lab2.Models;

namespace Web_Lab2.Repos;

public interface IOrderRep : IRep<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
    Task<Order> GetOrderWithItemsAsync(int orderId);
}