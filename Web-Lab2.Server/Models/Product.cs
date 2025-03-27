namespace Web_Lab2.Models;

public class Product
{
    public int Id { get; set; }
    public string? ProductNumber { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public bool IsDiscontinued { get; set; }
}