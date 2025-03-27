using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_Lab2.DTOs;
using Web_Lab2.Models;
using Web_Lab2.Repos;

namespace Web_Lab2.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        var products = await unitOfWork.Products.GetAllAsync();
        var productDTOs = new List<ProductDTO>();

        foreach (var product in products)
        {
            productDTOs.Add(new ProductDTO
            {
                Id = product.Id,
                ProductNumber = product.ProductNumber,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                IsDiscontinued = product.IsDiscontinued
            });
        }
        return Ok(productDTOs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var productDTO = new ProductDTO
        {
            Id = product.Id,
            ProductNumber = product.ProductNumber,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            IsDiscontinued = product.IsDiscontinued
        };

        return Ok(productDTO);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts([FromQuery] string name)
    {
        var products = await unitOfWork.Products.SearchByNameAsync(name);
        var productDTOs = new List<ProductDTO>();

        foreach (var product in products)
        {
            productDTOs.Add(new ProductDTO
            {
                Id = product.Id,
                ProductNumber = product.ProductNumber,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                IsDiscontinued = product.IsDiscontinued
            });
        }

        return Ok(productDTOs);
    }

    [HttpGet("number/{productNumber}")]
    public async Task<ActionResult<ProductDTO>> GetByProductNumber(string productNumber)
    {
        var product = await unitOfWork.Products.GetByProductNumberAsync(productNumber);

        if (product == null)
        {
            return NotFound();
        }

        var productDTO = new ProductDTO
        {
            Id = product.Id,
            ProductNumber = product.ProductNumber,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            IsDiscontinued = product.IsDiscontinued
        };

        return Ok(productDTO);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO createProductDTO)
    {
        var product = new Product
        {
            ProductNumber = createProductDTO.ProductNumber,
            Name = createProductDTO.Name,
            Description = createProductDTO.Description,
            Price = createProductDTO.Price,
            Category = createProductDTO.Category,
            IsDiscontinued = false
        };

        await unitOfWork.Products.AddAsync(product);
        await unitOfWork.CompleteAsync();

        var productDTO = new ProductDTO
        {
            Id = product.Id,
            ProductNumber = product.ProductNumber,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category,
            IsDiscontinued = product.IsDiscontinued
        };

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTO);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDTO updateProductDTO)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = updateProductDTO.Name;
        product.Description = updateProductDTO.Description;
        product.Price = updateProductDTO.Price;
        product.Category = updateProductDTO.Category;
        product.IsDiscontinued = updateProductDTO.IsDiscontinued;

        await unitOfWork.Products.UpdateAsync(product);
        await unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);
    
        if (product == null)
        {
            return NotFound();
        }

        var orderItems = await unitOfWork.OrderItems.GetItemsByProductIdAsync(id);
    
        if (orderItems.Any())
        {
            product.IsDiscontinued = true;
            await unitOfWork.Products.UpdateAsync(product);
        }
        else
        {
            await unitOfWork.Products.DeleteAsync(id);
        }
    
        await unitOfWork.CompleteAsync();

        return NoContent();
    }
}