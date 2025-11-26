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
    public async Task<IActionResult> CreateCustomer([FromBody] System.Text.Json.JsonElement jsonElement)
    {
        Customer customer = new Customer();
        
        try
        {
            // Log the raw JSON for debugging
            var rawJson = jsonElement.GetRawText();
            Console.WriteLine($"Raw JSON received: {rawJson}");
            
            // Manually extract ALL properties from JSON (bypass naming policy completely)
            // This ensures we get the data regardless of naming convention
            
            // First Name - try all variations
            if (jsonElement.TryGetProperty("FirstName", out var fn1))
                customer.FirstName = fn1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("first_name", out var fn2))
                customer.FirstName = fn2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("firstName", out var fn3))
                customer.FirstName = fn3.GetString() ?? string.Empty;
            Console.WriteLine($"FirstName extracted: '{customer.FirstName}' (length: {customer.FirstName.Length})");
            
            // Last Name - try all variations
            if (jsonElement.TryGetProperty("LastName", out var ln1))
                customer.LastName = ln1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("last_name", out var ln2))
                customer.LastName = ln2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("lastName", out var ln3))
                customer.LastName = ln3.GetString() ?? string.Empty;
            Console.WriteLine($"LastName extracted: '{customer.LastName}' (length: {customer.LastName.Length})");
            
            // Email - try all variations
            if (jsonElement.TryGetProperty("Email", out var em1))
                customer.Email = em1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("email", out var em2))
                customer.Email = em2.GetString() ?? string.Empty;
            Console.WriteLine($"Email extracted: '{customer.Email}'");
            
            // Password - CRITICAL - try all variations
            if (jsonElement.TryGetProperty("UserPassword", out var pw1))
            {
                customer.UserPassword = pw1.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'UserPassword': {customer.UserPassword.Length} chars");
            }
            else if (jsonElement.TryGetProperty("userpassword", out var pw2))
            {
                customer.UserPassword = pw2.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'userpassword': {customer.UserPassword.Length} chars");
            }
            else if (jsonElement.TryGetProperty("user_password", out var pw3))
            {
                customer.UserPassword = pw3.GetString() ?? string.Empty;
                Console.WriteLine($"✓ Password found in 'user_password': {customer.UserPassword.Length} chars");
            }
            else
            {
                Console.WriteLine("✗ Password property NOT FOUND!");
                // List all available properties with their values (except password)
                var props = new List<string>();
                foreach (var prop in jsonElement.EnumerateObject())
                {
                    var value = prop.Name.ToLower().Contains("password") ? "***" : prop.Value.ToString();
                    props.Add($"{prop.Name}={value}");
                }
                Console.WriteLine($"Available properties: {string.Join(", ", props)}");
            }
            
            // Phone
            if (jsonElement.TryGetProperty("Phone", out var ph1))
                customer.Phone = ph1.GetString();
            else if (jsonElement.TryGetProperty("phone", out var ph2))
                customer.Phone = ph2.GetString();
            
            // Address
            if (jsonElement.TryGetProperty("Address", out var ad1))
                customer.Address = ad1.GetString();
            else if (jsonElement.TryGetProperty("address", out var ad2))
                customer.Address = ad2.GetString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting customer data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "Invalid customer data format", error = ex.Message });
        }
        
        // Debug: Log what we extracted
        Console.WriteLine($"Extracted customer - FirstName: '{customer.FirstName}', LastName: '{customer.LastName}', Email: '{customer.Email}', Password: {(string.IsNullOrEmpty(customer.UserPassword) ? "EMPTY" : "SET (" + customer.UserPassword.Length + " chars)")}");
        
        // Validate required fields manually
        if (string.IsNullOrWhiteSpace(customer.FirstName))
        {
            Console.WriteLine("Validation failed: FirstName is empty");
            return BadRequest(new { message = "First name is required" });
        }
        if (string.IsNullOrWhiteSpace(customer.LastName))
        {
            Console.WriteLine("Validation failed: LastName is empty");
            return BadRequest(new { message = "Last name is required" });
        }
        if (string.IsNullOrWhiteSpace(customer.Email))
        {
            Console.WriteLine("Validation failed: Email is empty");
            return BadRequest(new { message = "Email is required" });
        }
        if (string.IsNullOrWhiteSpace(customer.UserPassword))
        {
            Console.WriteLine("Validation failed: UserPassword is empty");
            return BadRequest(new { message = "Password is required" });
        }
        
        try
        {
            var id = await _db.CreateCustomerAsync(customer);
            Console.WriteLine($"Customer created successfully with ID: {id}");
            return CreatedAtAction(nameof(GetCustomer), new { id }, customer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating customer: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Error creating customer", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] System.Text.Json.JsonElement jsonElement)
    {
        Customer customer = new Customer();
        
        try
        {
            // Manually extract properties to handle naming conventions
            var rawJson = jsonElement.GetRawText();
            Console.WriteLine($"Raw customer update JSON received: {rawJson}");
            
            // First Name - try all variations
            if (jsonElement.TryGetProperty("FirstName", out var fn1))
                customer.FirstName = fn1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("first_name", out var fn2))
                customer.FirstName = fn2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("firstName", out var fn3))
                customer.FirstName = fn3.GetString() ?? string.Empty;
            
            // Last Name - try all variations
            if (jsonElement.TryGetProperty("LastName", out var ln1))
                customer.LastName = ln1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("last_name", out var ln2))
                customer.LastName = ln2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("lastName", out var ln3))
                customer.LastName = ln3.GetString() ?? string.Empty;
            
            // Email - try all variations
            if (jsonElement.TryGetProperty("Email", out var em1))
                customer.Email = em1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("email", out var em2))
                customer.Email = em2.GetString() ?? string.Empty;
            
            // Password - try all variations
            if (jsonElement.TryGetProperty("UserPassword", out var pw1))
                customer.UserPassword = pw1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("userpassword", out var pw2))
                customer.UserPassword = pw2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("user_password", out var pw3))
                customer.UserPassword = pw3.GetString() ?? string.Empty;
            
            // Phone
            if (jsonElement.TryGetProperty("Phone", out var ph1))
            {
                if (ph1.ValueKind == System.Text.Json.JsonValueKind.Null)
                    customer.Phone = null;
                else
                    customer.Phone = ph1.GetString();
            }
            else if (jsonElement.TryGetProperty("phone", out var ph2))
            {
                if (ph2.ValueKind == System.Text.Json.JsonValueKind.Null)
                    customer.Phone = null;
                else
                    customer.Phone = ph2.GetString();
            }
            
            // Address
            if (jsonElement.TryGetProperty("Address", out var ad1))
            {
                if (ad1.ValueKind == System.Text.Json.JsonValueKind.Null)
                    customer.Address = null;
                else
                    customer.Address = ad1.GetString();
            }
            else if (jsonElement.TryGetProperty("address", out var ad2))
            {
                if (ad2.ValueKind == System.Text.Json.JsonValueKind.Null)
                    customer.Address = null;
                else
                    customer.Address = ad2.GetString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting customer update data: {ex.Message}");
            return BadRequest(new { message = "Invalid customer data format", error = ex.Message });
        }
        
        // Validate required fields
        if (string.IsNullOrWhiteSpace(customer.FirstName))
        {
            return BadRequest(new { message = "First name is required" });
        }
        
        if (string.IsNullOrWhiteSpace(customer.LastName))
        {
            return BadRequest(new { message = "Last name is required" });
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
