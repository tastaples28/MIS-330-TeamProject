namespace CampusReHome.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string UserPassword { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime JoinDate { get; set; }
}

