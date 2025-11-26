using Microsoft.AspNetCore.Mvc;
using CampusReHome.Models;
using CampusReHome.Services;

namespace CampusReHome.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseService _db;

    public AuthController(DatabaseService db)
    {
        _db = db;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required" });
        }

        try
        {
            if (request.UserType == "customer")
            {
                var customers = await _db.GetAllCustomersAsync();
                var customer = customers.FirstOrDefault(c => 
                    c.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && 
                    c.UserPassword == request.Password);

                if (customer == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                return Ok(new
                {
                    token = $"customer_{customer.CustomerId}",
                    userId = customer.CustomerId,
                    name = $"{customer.FirstName} {customer.LastName}",
                    userType = "customer"
                });
            }
            else if (request.UserType == "employee")
            {
                var employees = await _db.GetAllEmployeesAsync();
                var employee = employees.FirstOrDefault(e => 
                    e.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && 
                    e.UserPassword == request.Password &&
                    e.IsActive);

                if (employee == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                return Ok(new
                {
                    token = $"employee_{employee.EmployeeId}",
                    userId = employee.EmployeeId,
                    name = $"{employee.FirstName} {employee.LastName}",
                    userType = "employee"
                });
            }
            else
            {
                return BadRequest(new { message = "Invalid user type" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Client-side logout (localStorage cleared)
        return Ok(new { message = "Logged out successfully" });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}

