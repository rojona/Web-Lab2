namespace Web_Lab2.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    public string? ProductNumber { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public bool IsDiscontinued { get; set; }
}

public class CreateProductDTO
{
    public string? ProductNumber { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
}

public class UpdateProductDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public bool IsDiscontinued { get; set; }
}