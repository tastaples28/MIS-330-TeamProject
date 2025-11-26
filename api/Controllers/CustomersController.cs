using Microsoft.AspNetCore.Mvc;
using CampusReHome.Models;
using CampusReHome.Services;

namespace CampusReHome.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly DatabaseService _db;

    public CustomersController(DatabaseService db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] string? format)
    {
        // Only respond to API requests (with format=json)
        // Regular page requests should be handled by HTML routes
        if (format != "json")
        {
            return NotFound(); // Let HTML route handle it
        }
        
        var customers = await _db.GetAllCustomersAsync();
        return Ok(new { customers });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _db.GetCustomerByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var id = await _db.CreateCustomerAsync(customer);
        return CreatedAtAction(nameof(GetCustomer), new { id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(new { message = "Invalid model state", errors = ModelState });
        }
        
        if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName))
        {
            return BadRequest(new { message = "First name and last name are required" });
        }
        
        var updated = await _db.UpdateCustomerAsync(id, customer);
        if (!updated) 
        {
            return NotFound(new { message = $"Customer with id {id} not found" });
        }
        
        // Return the updated customer instead of NoContent
        var updatedCustomer = await _db.GetCustomerByIdAsync(id);
        return Ok(updatedCustomer);
    }

    [HttpDelete("{id}")]
    [HttpPost("{id}/delete")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await _db.DeleteCustomerAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}

