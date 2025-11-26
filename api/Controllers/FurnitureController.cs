using Microsoft.AspNetCore.Mvc;
using CampusReHome.Models;
using CampusReHome.Services;

namespace CampusReHome.Controllers;

[ApiController]
[Route("api/furniture")]
public class FurnitureController : ControllerBase
{
    private readonly DatabaseService _db;

    public FurnitureController(DatabaseService db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetFurniture([FromQuery] string? format, [FromQuery] string? search, [FromQuery] string? category)
    {
        // Only respond to API requests (with format=json)
        if (format != "json")
        {
            return NotFound(); // Let HTML route handle it
        }
        
        var furniture = await _db.GetAllFurnitureAsync(search, category);
        var categories = await _db.GetCategoriesAsync();
        return Ok(new { furniture, categories });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFurnitureItem(int id)
    {
        var item = await _db.GetFurnitureByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFurniture([FromBody] System.Text.Json.JsonElement jsonElement)
    {
        Furniture furniture = new Furniture();
        
        try
        {
            // Manually extract properties to handle naming conventions
            var rawJson = jsonElement.GetRawText();
            Console.WriteLine($"Raw furniture JSON received: {rawJson}");
            
            // ItemName - try all variations
            if (jsonElement.TryGetProperty("ItemName", out var in1))
                furniture.ItemName = in1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("item_name", out var in2))
                furniture.ItemName = in2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("itemName", out var in3))
                furniture.ItemName = in3.GetString() ?? string.Empty;
            
            Console.WriteLine($"ItemName extracted: {furniture.ItemName}");
            
            // Category - try all variations
            if (jsonElement.TryGetProperty("Category", out var cat1))
                furniture.Category = cat1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("category", out var cat2))
                furniture.Category = cat2.GetString() ?? string.Empty;
            
            // Description - try all variations
            if (jsonElement.TryGetProperty("Description", out var desc1))
            {
                if (desc1.ValueKind == System.Text.Json.JsonValueKind.Null)
                    furniture.Description = null;
                else
                    furniture.Description = desc1.GetString();
            }
            else if (jsonElement.TryGetProperty("description", out var desc2))
            {
                if (desc2.ValueKind == System.Text.Json.JsonValueKind.Null)
                    furniture.Description = null;
                else
                    furniture.Description = desc2.GetString();
            }
            
            // ItemCondition - try all variations
            if (jsonElement.TryGetProperty("ItemCondition", out var cond1))
                furniture.ItemCondition = cond1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("item_condition", out var cond2))
                furniture.ItemCondition = cond2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("itemCondition", out var cond3))
                furniture.ItemCondition = cond3.GetString() ?? string.Empty;
            
            // Price - try all variations
            if (jsonElement.TryGetProperty("Price", out var price1))
                furniture.Price = price1.GetDecimal();
            else if (jsonElement.TryGetProperty("price", out var price2))
                furniture.Price = price2.GetDecimal();
            
            // StockQuantity - try all variations
            if (jsonElement.TryGetProperty("StockQuantity", out var stock1))
                furniture.StockQuantity = stock1.GetInt32();
            else if (jsonElement.TryGetProperty("stock_quantity", out var stock2))
                furniture.StockQuantity = stock2.GetInt32();
            else if (jsonElement.TryGetProperty("stockQuantity", out var stock3))
                furniture.StockQuantity = stock3.GetInt32();
            
            Console.WriteLine($"StockQuantity extracted: {furniture.StockQuantity}");
            
            // IsActive - try all variations
            if (jsonElement.TryGetProperty("IsActive", out var active1))
                furniture.IsActive = active1.GetBoolean();
            else if (jsonElement.TryGetProperty("is_active", out var active2))
                furniture.IsActive = active2.GetBoolean();
            else if (jsonElement.TryGetProperty("isActive", out var active3))
                furniture.IsActive = active3.GetBoolean();
            else
                furniture.IsActive = true; // Default to true
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting furniture data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "Invalid furniture data format", error = ex.Message });
        }
        
        // Validate required fields
        if (string.IsNullOrWhiteSpace(furniture.ItemName))
        {
            return BadRequest(new { message = "Item name is required" });
        }
        
        if (string.IsNullOrWhiteSpace(furniture.Category))
        {
            return BadRequest(new { message = "Category is required" });
        }
        
        if (string.IsNullOrWhiteSpace(furniture.ItemCondition))
        {
            return BadRequest(new { message = "Item condition is required" });
        }
        
        try
        {
            var id = await _db.CreateFurnitureAsync(furniture);
            furniture.FurnitureId = id;
            return CreatedAtAction(nameof(GetFurnitureItem), new { id }, furniture);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating furniture: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Error creating furniture", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFurniture(int id, [FromBody] System.Text.Json.JsonElement jsonElement)
    {
        Furniture furniture = new Furniture();
        
        try
        {
            // Manually extract properties to handle naming conventions
            // ItemName - try all variations
            if (jsonElement.TryGetProperty("ItemName", out var in1))
                furniture.ItemName = in1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("item_name", out var in2))
                furniture.ItemName = in2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("itemName", out var in3))
                furniture.ItemName = in3.GetString() ?? string.Empty;
            
            // Category - try all variations
            if (jsonElement.TryGetProperty("Category", out var cat1))
                furniture.Category = cat1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("category", out var cat2))
                furniture.Category = cat2.GetString() ?? string.Empty;
            
            // Description - try all variations
            if (jsonElement.TryGetProperty("Description", out var desc1))
            {
                if (desc1.ValueKind == System.Text.Json.JsonValueKind.Null)
                    furniture.Description = null;
                else
                    furniture.Description = desc1.GetString();
            }
            else if (jsonElement.TryGetProperty("description", out var desc2))
            {
                if (desc2.ValueKind == System.Text.Json.JsonValueKind.Null)
                    furniture.Description = null;
                else
                    furniture.Description = desc2.GetString();
            }
            
            // ItemCondition - try all variations
            if (jsonElement.TryGetProperty("ItemCondition", out var cond1))
                furniture.ItemCondition = cond1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("item_condition", out var cond2))
                furniture.ItemCondition = cond2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("itemCondition", out var cond3))
                furniture.ItemCondition = cond3.GetString() ?? string.Empty;
            
            // Price - try all variations
            if (jsonElement.TryGetProperty("Price", out var price1))
                furniture.Price = price1.GetDecimal();
            else if (jsonElement.TryGetProperty("price", out var price2))
                furniture.Price = price2.GetDecimal();
            
            // StockQuantity - try all variations
            if (jsonElement.TryGetProperty("StockQuantity", out var stock1))
                furniture.StockQuantity = stock1.GetInt32();
            else if (jsonElement.TryGetProperty("stock_quantity", out var stock2))
                furniture.StockQuantity = stock2.GetInt32();
            else if (jsonElement.TryGetProperty("stockQuantity", out var stock3))
                furniture.StockQuantity = stock3.GetInt32();
            
            // IsActive - try all variations
            if (jsonElement.TryGetProperty("IsActive", out var active1))
                furniture.IsActive = active1.GetBoolean();
            else if (jsonElement.TryGetProperty("is_active", out var active2))
                furniture.IsActive = active2.GetBoolean();
            else if (jsonElement.TryGetProperty("isActive", out var active3))
                furniture.IsActive = active3.GetBoolean();
            else
                furniture.IsActive = true; // Default to true
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting furniture data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "Invalid furniture data format", error = ex.Message });
        }
        
        if (string.IsNullOrWhiteSpace(furniture.ItemName))
        {
            return BadRequest(new { message = "Item name is required" });
        }
        
        var updated = await _db.UpdateFurnitureAsync(id, furniture);
        if (!updated) 
        {
            return NotFound(new { message = $"Furniture with id {id} not found" });
        }
        
        // Return the updated furniture instead of NoContent
        var updatedFurniture = await _db.GetFurnitureByIdAsync(id);
        return Ok(updatedFurniture);
    }

    [HttpPost("{id}/delete")]
    public async Task<IActionResult> DeleteFurniture(int id)
    {
        var deleted = await _db.DeleteFurnitureAsync(id);
        if (!deleted) return NotFound();
        return Ok(new { success = true });
    }
}

