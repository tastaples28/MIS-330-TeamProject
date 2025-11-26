using Microsoft.AspNetCore.Mvc;
using CampusReHome.Models;
using CampusReHome.Services;

namespace CampusReHome.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly DatabaseService _db;

    public EmployeesController(DatabaseService db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] string? format)
    {
        // Only respond to API requests (with format=json)
        if (format != "json")
        {
            return NotFound(); // Let HTML route handle it
        }
        
        var employees = await _db.GetAllEmployeesAsync();
        return Ok(new { employees });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _db.GetEmployeeByIdAsync(id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
    {
        // Debug: Log what we received
        Console.WriteLine($"Received employee: FirstName={employee?.FirstName}, LastName={employee?.LastName}, Email={employee?.Email}, Password={(employee?.UserPassword != null ? "***" : "null")}");
        
        // Validate required fields manually
        if (employee == null)
        {
            return BadRequest(new { message = "Employee data is required" });
        }
        
        if (string.IsNullOrWhiteSpace(employee.FirstName))
        {
            return BadRequest(new { message = "First name is required" });
        }
        
        if (string.IsNullOrWhiteSpace(employee.LastName))
        {
            return BadRequest(new { message = "Last name is required" });
        }
        
        if (string.IsNullOrWhiteSpace(employee.Email))
        {
            return BadRequest(new { message = "Email is required" });
        }
        
        if (string.IsNullOrWhiteSpace(employee.UserPassword))
        {
            return BadRequest(new { message = "Password is required" });
        }
        
        if (string.IsNullOrWhiteSpace(employee.Role))
        {
            return BadRequest(new { message = "Role is required" });
        }
        
        try
        {
            var id = await _db.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id }, employee);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee: {ex.Message}");
            return StatusCode(500, new { message = "Error creating employee", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(new { message = "Invalid model state", errors = ModelState });
        }
        
        if (string.IsNullOrWhiteSpace(employee.FirstName) || string.IsNullOrWhiteSpace(employee.LastName))
        {
            return BadRequest(new { message = "First name and last name are required" });
        }
        
        var updated = await _db.UpdateEmployeeAsync(id, employee);
        if (!updated) 
        {
            return NotFound(new { message = $"Employee with id {id} not found" });
        }
        
        // Return the updated employee instead of NoContent
        var updatedEmployee = await _db.GetEmployeeByIdAsync(id);
        return Ok(updatedEmployee);
    }

    [HttpPost("{id}/delete")]
    public async Task<IActionResult> DeactivateEmployee(int id)
    {
        var deactivated = await _db.DeactivateEmployeeAsync(id);
        if (!deactivated) return NotFound();
        return Ok(new { success = true });
    }
}

