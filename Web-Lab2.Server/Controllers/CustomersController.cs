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
public class CustomersController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
    {
        var customers = await unitOfWork.Customers.GetAllAsync();
        var customerDTOs = new List<CustomerDTO>();

        foreach (var customer in customers)
        {
            customerDTOs.Add(new CustomerDTO
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            });
        }

        return Ok(customerDTOs);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        
        var customer = await unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin") && customer.Email != userEmail)
        {
            return Forbid();
        }

        var customerDTO = new CustomerDTO
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country
        };

        return Ok(customerDTO);
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<ActionResult<CustomerDTO>> GetByEmail(string email)
    {
        if (!User.IsInRole("Admin"))
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email != userEmail)
            {
                return Forbid();
            }
        }

        var customer = await unitOfWork.Customers.GetByEmailAsync(email);

        if (customer == null)
        {
            return NotFound();
        }

        var customerDTO = new CustomerDTO
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country
        };

        return Ok(customerDTO);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CustomerDTO>> CreateCustomer(CreateCustomerDTO createCustomerDTO)
    {
        try
        {
            var existingCustomer = await unitOfWork.Customers.GetByEmailAsync(createCustomerDTO.Email);
            if (existingCustomer != null)
            {
                return BadRequest($"A customer with email {createCustomerDTO.Email} already exists.");
            }

            var customer = new Customer
            {
                FirstName = createCustomerDTO.FirstName,
                LastName = createCustomerDTO.LastName,
                Email = createCustomerDTO.Email,
                Phone = createCustomerDTO.Phone,
                Address = createCustomerDTO.Address,
                City = createCustomerDTO.City,
                PostalCode = createCustomerDTO.PostalCode,
                Country = createCustomerDTO.Country
            };

            await unitOfWork.Customers.AddAsync(customer);
            await unitOfWork.CompleteAsync();

            var customerDTO = new CustomerDTO
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            };

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customerDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDTO updateCustomerDTO)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        
        var customer = await unitOfWork.Customers.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }
        
        if (!User.IsInRole("Admin") && customer.Email != userEmail)
        {
            return Forbid();
        }

        customer.FirstName = updateCustomerDTO.FirstName;
        customer.LastName = updateCustomerDTO.LastName;
        customer.Phone = updateCustomerDTO.Phone;
        customer.Address = updateCustomerDTO.Address;
        customer.City = updateCustomerDTO.City;
        customer.PostalCode = updateCustomerDTO.PostalCode;
        customer.Country = updateCustomerDTO.Country;

        await unitOfWork.Customers.UpdateAsync(customer);
        await unitOfWork.CompleteAsync();

        return NoContent();
    }
}