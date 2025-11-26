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
    public async Task<IActionResult> CreateEmployee([FromBody] System.Text.Json.JsonElement jsonElement)
    {
        Employee employee = new Employee();
        
        try
        {
            // Log the raw JSON for debugging
            var rawJson = jsonElement.GetRawText();
            Console.WriteLine($"Raw JSON received: {rawJson}");
            
            // Manually extract ALL properties from JSON (bypass naming policy completely)
            // This ensures we get the data regardless of naming convention
            
            // First Name - try all variations
            if (jsonElement.TryGetProperty("FirstName", out var fn1))
                employee.FirstName = fn1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("first_name", out var fn2))
                employee.FirstName = fn2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("firstName", out var fn3))
                employee.FirstName = fn3.GetString() ?? string.Empty;
            Console.WriteLine($"FirstName extracted: '{employee.FirstName}' (length: {employee.FirstName.Length})");
            
            // Last Name - try all variations
            if (jsonElement.TryGetProperty("LastName", out var ln1))
                employee.LastName = ln1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("last_name", out var ln2))
                employee.LastName = ln2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("lastName", out var ln3))
                employee.LastName = ln3.GetString() ?? string.Empty;
            Console.WriteLine($"LastName extracted: '{employee.LastName}' (length: {employee.LastName.Length})");
            
            // Email - try all variations
            if (jsonElement.TryGetProperty("Email", out var em1))
                employee.Email = em1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("email", out var em2))
                employee.Email = em2.GetString() ?? string.Empty;
            Console.WriteLine($"Email extracted: '{employee.Email}'");
            
            // Password - CRITICAL - try all variations
            if (jsonElement.TryGetProperty("UserPassword", out var pw1))
            {
                employee.UserPassword = pw1.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'UserPassword': {employee.UserPassword.Length} chars");
            }
            else if (jsonElement.TryGetProperty("userpassword", out var pw2))
            {
                employee.UserPassword = pw2.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'userpassword': {employee.UserPassword.Length} chars");
            }
            else if (jsonElement.TryGetProperty("user_password", out var pw3))
            {
                employee.UserPassword = pw3.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'user_password': {employee.UserPassword.Length} chars");
            }
            else
            {
                Console.WriteLine("✗ Password property NOT FOUND!");
            }
            
            // Phone Number
            if (jsonElement.TryGetProperty("PhoneNumber", out var ph1))
                employee.PhoneNumber = ph1.GetString();
            else if (jsonElement.TryGetProperty("phone_number", out var ph2))
                employee.PhoneNumber = ph2.GetString();
            else if (jsonElement.TryGetProperty("phoneNumber", out var ph3))
                employee.PhoneNumber = ph3.GetString();
            
            // Role
            if (jsonElement.TryGetProperty("Role", out var r1))
                employee.Role = r1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("role", out var r2))
                employee.Role = r2.GetString() ?? string.Empty;
            
            // Hire Date
            if (jsonElement.TryGetProperty("HireDate", out var hd1))
            {
                if (hd1.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(hd1.GetString(), out var date1))
                    employee.HireDate = date1;
            }
            else if (jsonElement.TryGetProperty("hire_date", out var hd2))
            {
                if (hd2.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(hd2.GetString(), out var date2))
                    employee.HireDate = date2;
            }
            else if (jsonElement.TryGetProperty("hireDate", out var hd3))
            {
                if (hd3.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(hd3.GetString(), out var date3))
                    employee.HireDate = date3;
            }
            
            // Is Active
            if (jsonElement.TryGetProperty("IsActive", out var ia1))
                employee.IsActive = ia1.GetBoolean();
            else if (jsonElement.TryGetProperty("is_active", out var ia2))
                employee.IsActive = ia2.GetBoolean();
            else if (jsonElement.TryGetProperty("isActive", out var ia3))
                employee.IsActive = ia3.GetBoolean();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting employee data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "Invalid employee data format", error = ex.Message });
        }
        
        // Debug: Log what we extracted
        Console.WriteLine($"Extracted employee - FirstName: '{employee.FirstName}', LastName: '{employee.LastName}', Email: '{employee.Email}', Password: {(string.IsNullOrEmpty(employee.UserPassword) ? "EMPTY" : "SET (" + employee.UserPassword.Length + " chars)")}");
        
        // Validate required fields manually
        if (string.IsNullOrWhiteSpace(employee.FirstName))
        {
            Console.WriteLine("Validation failed: FirstName is empty");
            return BadRequest(new { message = "First name is required" });
        }
        if (string.IsNullOrWhiteSpace(employee.LastName))
        {
            Console.WriteLine("Validation failed: LastName is empty");
            return BadRequest(new { message = "Last name is required" });
        }
        if (string.IsNullOrWhiteSpace(employee.Email))
        {
            Console.WriteLine("Validation failed: Email is empty");
            return BadRequest(new { message = "Email is required" });
        }
        if (string.IsNullOrWhiteSpace(employee.UserPassword))
        {
            Console.WriteLine("Validation failed: UserPassword is empty");
            return BadRequest(new { message = "Password is required" });
        }
        if (string.IsNullOrWhiteSpace(employee.Role))
        {
            Console.WriteLine("Validation failed: Role is empty");
            return BadRequest(new { message = "Role is required" });
        }
        
        try
        {
            var id = await _db.CreateEmployeeAsync(employee);
            Console.WriteLine($"Employee created successfully with ID: {id}");
            return CreatedAtAction(nameof(GetEmployee), new { id }, employee);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

