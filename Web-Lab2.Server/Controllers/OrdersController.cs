using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_Lab2.DTOs;
using Web_Lab2.Models;
using Web_Lab2.Repos;

namespace Web_Lab2.Controllers;

[Route("api/[controller]")]
[ApiController]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class OrdersController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var orders = await unitOfWork.Orders.GetAllAsync();
        var orderDTOs = new List<OrderDTO>();

        foreach (var order in orders)
        {
            var orderWithItems = await unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
            orderDTOs.Add(MapOrderToDTO(orderWithItems));
        }

        return Ok(orderDTOs);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var order = await unitOfWork.Orders.GetOrderWithItemsAsync(id);

        if (order == null)
        {
            return NotFound();
        }
        
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var customer = await unitOfWork.Customers.GetByIdAsync(order.CustomerId);
        
        if (!User.IsInRole("Admin") && customer.Email != userEmail)
        {
            return Forbid();
        }

        var orderDTO = MapOrderToDTO(order);

        return Ok(orderDTO);
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByCustomer(int customerId)
    {
        var customer = await unitOfWork.Customers.GetByIdAsync(customerId);
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        
        if (customer == null)
        {
            return NotFound("Customer not found.");
        }
        
        if (!User.IsInRole("Admin") && customer.Email != userEmail)
        {
            return Forbid();
        }

        var orders = await unitOfWork.Orders.GetOrdersByCustomerIdAsync(customerId);
        var orderDTOs = new List<OrderDTO>();

        foreach (var order in orders)
        {
            await unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
            orderDTOs.Add(MapOrderToDTO(order));
        }

        return Ok(orderDTOs);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OrderDTO>> CreateOrder(OrderItemDTO.CreateOrderDTO createOrderDTO)
    {
        var customer = await unitOfWork.Customers.GetByIdAsync(createOrderDTO.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found.");
        }
    
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
    
        if (!User.IsInRole("Admin") && customer.Email != userEmail)
        {
            return Forbid();
        }

        foreach (var item in createOrderDTO.OrderItems)
        {
            var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                return BadRequest($"Product with ID {item.ProductId} not found.");
            }
            if (product.IsDiscontinued)
            {
                return BadRequest($"Product {item.ProductId} is discontinued.");
            }
        }

        var order = new Order
        {
            CustomerId = createOrderDTO.CustomerId,
            OrderDate = DateTime.Now,
            Customer = customer
        };

        foreach (var item in createOrderDTO.OrderItems)
        {
            var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
        
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                ProductName = product.Name,
                ProductNumber = product.ProductNumber,
                ProductDescription = product.Description,
                ProductCategory = product.Category,
                Product = product
            };
        
            order.OrderItems.Add(orderItem);
        }

        await unitOfWork.Orders.AddAsync(order);
        await unitOfWork.CompleteAsync();

        var createdOrder = await unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
        var orderDTO = MapOrderToDTO(createdOrder);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDTO);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }

        await unitOfWork.Orders.DeleteAsync(id);
        await unitOfWork.CompleteAsync();

        return NoContent();
    }

    private OrderDTO MapOrderToDTO(Order order)
    {
        var orderDTO = new OrderDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = $"{order.Customer.FirstName} {order.Customer.LastName}",
            OrderDate = order.OrderDate,
            TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
        };

        foreach (var item in order.OrderItems)
        {
            orderDTO.OrderItems.Add(new OrderItemDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductNumber = item.ProductNumber,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            });
        }

        return orderDTO;
    }
}