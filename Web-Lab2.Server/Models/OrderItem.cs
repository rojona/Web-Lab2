namespace Web_Lab2.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public string? ProductName { get; set; }
    public string? ProductNumber { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductCategory { get; set; }
    
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}