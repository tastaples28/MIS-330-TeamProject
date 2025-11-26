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
    public async Task<IActionResult> CreateFurniture([FromBody] Furniture furniture)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var id = await _db.CreateFurnitureAsync(furniture);
        return CreatedAtAction(nameof(GetFurnitureItem), new { id }, furniture);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFurniture(int id, [FromBody] Furniture furniture)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(new { message = "Invalid model state", errors = ModelState });
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

