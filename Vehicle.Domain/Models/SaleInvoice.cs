using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class SaleInvoice
{
    [Key] public int InvoiceID { get; set; }

    [Required, MaxLength(40)] public string InvoiceNumber { get; set; } = string.Empty;

    public int CustomerID { get; set; }
    [ForeignKey("CustomerID")] public Customer Customer { get; set; } = null!;

    public int StaffID { get; set; }
    [ForeignKey("StaffID")] public Staff Staff { get; set; } = null!;

    [Column(TypeName = "numeric(12,2)")] public decimal SubTotal { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal Discount { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal Tax { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal TotalAmount { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal AmountPaid { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal AmountDue { get; set; }

    [Required, MaxLength(20)] public string PaymentStatus { get; set; } = "Paid"; // Paid / PartiallyPaid / Credit
    [MaxLength(30)] public string? PaymentMethod { get; set; } // Cash, eSewa, Khalti, Bank, Credit

    [MaxLength(500)] public string? Notes { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? LastReminderSentAt { get; set; }

    public ICollection<SaleInvoiceItem> Items { get; set; } = new List<SaleInvoiceItem>();
}
