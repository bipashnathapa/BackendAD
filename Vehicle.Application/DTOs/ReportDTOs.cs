namespace Vehicle.Application.DTOs;

public class RegularCustomerDTO
{
    public int CustomerID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int VisitCount { get; set; }
    public DateTime LastVisit { get; set; }
}

public class HighSpenderDTO
{
    public int CustomerID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public decimal TotalSpent { get; set; }
    public int InvoiceCount { get; set; }
}

public class PendingCreditDTO
{
    public int CustomerID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal TotalDue { get; set; }
    public int OverdueInvoiceCount { get; set; }
    public DateTime OldestUnpaidDate { get; set; }
    public int DaysOverdue { get; set; }
}
