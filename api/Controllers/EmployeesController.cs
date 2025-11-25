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
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var id = await _db.CreateEmployeeAsync(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id }, employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var updated = await _db.UpdateEmployeeAsync(id, employee);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/delete")]
    public async Task<IActionResult> DeactivateEmployee(int id)
    {
        var deactivated = await _db.DeactivateEmployeeAsync(id);
        if (!deactivated) return NotFound();
        return Ok(new { success = true });
    }
}

