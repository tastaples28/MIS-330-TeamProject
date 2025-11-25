namespace CampusReHome.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string UserPassword { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime? HireDate { get; set; }
    public bool IsActive { get; set; } = true;
}

