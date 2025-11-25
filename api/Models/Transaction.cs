namespace CampusReHome.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    // Joined data
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
    public string? EmployeeFirstName { get; set; }
    public string? EmployeeLastName { get; set; }
    
    // Transaction details
    public List<TransactionDetail>? Details { get; set; }
}

public class TransactionDetail
{
    public int TransactionDetailId { get; set; }
    public int TransactionId { get; set; }
    public int FurnitureId { get; set; }
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
    
    // Joined data
    public string? FurnitureName { get; set; }
}

