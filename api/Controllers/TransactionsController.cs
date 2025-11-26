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
    public async Task<IActionResult> CreateTransaction([FromBody] System.Text.Json.JsonElement jsonElement)
    {
        TransactionRequest request = new TransactionRequest();
        
        try
        {
            // Manually extract properties to handle naming conventions
            var rawJson = jsonElement.GetRawText();
            Console.WriteLine($"Raw transaction JSON received: {rawJson}");
            
            // CustomerId - try all variations
            if (jsonElement.TryGetProperty("CustomerId", out var cid1))
                request.CustomerId = cid1.GetInt32();
            else if (jsonElement.TryGetProperty("customer_id", out var cid2))
                request.CustomerId = cid2.GetInt32();
            else if (jsonElement.TryGetProperty("customerId", out var cid3))
                request.CustomerId = cid3.GetInt32();
            
            Console.WriteLine($"CustomerId extracted: {request.CustomerId}");
            
            // EmployeeId - try all variations
            if (jsonElement.TryGetProperty("EmployeeId", out var eid1))
                request.EmployeeId = eid1.GetInt32();
            else if (jsonElement.TryGetProperty("employee_id", out var eid2))
                request.EmployeeId = eid2.GetInt32();
            else if (jsonElement.TryGetProperty("employeeId", out var eid3))
                request.EmployeeId = eid3.GetInt32();
            
            Console.WriteLine($"EmployeeId extracted: {request.EmployeeId}");
            
            // TransactionDate
            if (jsonElement.TryGetProperty("TransactionDate", out var td1))
            {
                if (td1.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(td1.GetString(), out var date1))
                    request.TransactionDate = date1;
            }
            else if (jsonElement.TryGetProperty("transaction_date", out var td2))
            {
                if (td2.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(td2.GetString(), out var date2))
                    request.TransactionDate = date2;
            }
            else if (jsonElement.TryGetProperty("transactionDate", out var td3))
            {
                if (td3.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(td3.GetString(), out var date3))
                    request.TransactionDate = date3;
            }
            
            // TotalAmount
            if (jsonElement.TryGetProperty("TotalAmount", out var ta1))
                request.TotalAmount = ta1.GetDecimal();
            else if (jsonElement.TryGetProperty("total_amount", out var ta2))
                request.TotalAmount = ta2.GetDecimal();
            else if (jsonElement.TryGetProperty("totalAmount", out var ta3))
                request.TotalAmount = ta3.GetDecimal();
            
            // PaymentMethod
            if (jsonElement.TryGetProperty("PaymentMethod", out var pm1))
                request.PaymentMethod = pm1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("payment_method", out var pm2))
                request.PaymentMethod = pm2.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("paymentMethod", out var pm3))
                request.PaymentMethod = pm3.GetString() ?? string.Empty;
            
            // Status
            if (jsonElement.TryGetProperty("Status", out var s1))
                request.Status = s1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("status", out var s2))
                request.Status = s2.GetString() ?? string.Empty;
            
            // Details
            System.Text.Json.JsonElement detailsElement = default;
            bool hasDetails = false;
            if (jsonElement.TryGetProperty("Details", out var det1))
            {
                detailsElement = det1;
                hasDetails = true;
            }
            else if (jsonElement.TryGetProperty("details", out var det1Alt))
            {
                detailsElement = det1Alt;
                hasDetails = true;
            }
            
            if (hasDetails && detailsElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    request.Details = new List<TransactionDetailRequest>();
                    foreach (var detail in detailsElement.EnumerateArray())
                    {
                        var detailReq = new TransactionDetailRequest();
                        if (detail.TryGetProperty("FurnitureId", out var fid1))
                            detailReq.FurnitureId = fid1.GetInt32();
                        else if (detail.TryGetProperty("furniture_id", out var fid2))
                            detailReq.FurnitureId = fid2.GetInt32();
                        else if (detail.TryGetProperty("furnitureId", out var fid3))
                            detailReq.FurnitureId = fid3.GetInt32();
                        
                        if (detail.TryGetProperty("Quantity", out var q1))
                            detailReq.Quantity = q1.GetInt32();
                        else if (detail.TryGetProperty("quantity", out var q2))
                            detailReq.Quantity = q2.GetInt32();
                        
                        if (detail.TryGetProperty("SalePrice", out var sp1))
                            detailReq.SalePrice = sp1.GetDecimal();
                        else if (detail.TryGetProperty("sale_price", out var sp2))
                            detailReq.SalePrice = sp2.GetDecimal();
                        else if (detail.TryGetProperty("salePrice", out var sp3))
                            detailReq.SalePrice = sp3.GetDecimal();
                        
                        request.Details.Add(detailReq);
                    }
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting transaction data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "Invalid transaction data format", error = ex.Message });
        }
        
        // Validate required fields
        if (request.CustomerId == 0)
        {
            return BadRequest(new { message = "CustomerId is required and must be greater than 0" });
        }
        
        if (request.EmployeeId == 0)
        {
            return BadRequest(new { message = "EmployeeId is required and must be greater than 0" });
        }
        
        // Validate customer exists
        var customer = await _db.GetCustomerByIdAsync(request.CustomerId);
        if (customer == null)
        {
            return BadRequest(new { message = $"Customer with ID {request.CustomerId} not found. Please ensure you are logged in as a valid customer." });
        }
        
        // Validate employee exists
        var employee = await _db.GetEmployeeByIdAsync(request.EmployeeId);
        if (employee == null)
        {
            return BadRequest(new { message = $"Employee with ID {request.EmployeeId} not found." });
        }
        
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
        
        try
        {
            var id = await _db.CreateTransactionAsync(transaction, details);
            // Set the TransactionId on the transaction object so it's included in the response
            transaction.TransactionId = id;
            return CreatedAtAction(nameof(GetTransaction), new { id }, transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating transaction: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Error creating transaction", error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] System.Text.Json.JsonElement jsonElement)
    {
        string status = string.Empty;
        
        try
        {
            // Extract status from JSON (handle different naming conventions)
            if (jsonElement.TryGetProperty("Status", out var s1))
                status = s1.GetString() ?? string.Empty;
            else if (jsonElement.TryGetProperty("status", out var s2))
                status = s2.GetString() ?? string.Empty;
            
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { message = "Status is required" });
            }
            
            // Validate status value
            var validStatuses = new[] { "Pending", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { message = $"Invalid status. Must be one of: {string.Join(", ", validStatuses)}" });
            }
            
            var updated = await _db.UpdateTransactionStatusAsync(id, status);
            if (!updated)
            {
                return NotFound(new { message = "Transaction not found" });
            }
            
            // Return updated transaction
            var transaction = await _db.GetTransactionByIdAsync(id);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating transaction status: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Error updating transaction status", error = ex.Message });
        }
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

