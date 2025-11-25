using Microsoft.AspNetCore.Mvc;
using CampusReHome.Models;
using CampusReHome.Services;

namespace CampusReHome.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly DatabaseService _db;

    public TransactionsController(DatabaseService db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] string? format)
    {
        // Only respond to API requests (with format=json)
        if (format != "json")
        {
            return NotFound(); // Let HTML route handle it
        }
        
        var transactions = await _db.GetAllTransactionsAsync();
        return Ok(new { transactions });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        var transaction = await _db.GetTransactionByIdAsync(id);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var transaction = new Transaction
        {
            CustomerId = request.CustomerId,
            EmployeeId = request.EmployeeId,
            TransactionDate = request.TransactionDate,
            TotalAmount = request.TotalAmount,
            PaymentMethod = request.PaymentMethod,
            Status = request.Status
        };
        
        var details = request.Details.Select(d => new TransactionDetail
        {
            FurnitureId = d.FurnitureId,
            Quantity = d.Quantity,
            SalePrice = d.SalePrice
        }).ToList();
        
        var id = await _db.CreateTransactionAsync(transaction, details);
        return CreatedAtAction(nameof(GetTransaction), new { id }, transaction);
    }
}

public class TransactionRequest
{
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<TransactionDetailRequest> Details { get; set; } = new();
}

public class TransactionDetailRequest
{
    public int FurnitureId { get; set; }
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
}

