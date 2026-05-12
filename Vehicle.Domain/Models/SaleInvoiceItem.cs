using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle.Domain.Models;

public class SaleInvoiceItem
{
    [Key] public int SaleInvoiceItemID { get; set; }

    public int InvoiceID { get; set; }
    [ForeignKey("InvoiceID")] public SaleInvoice Invoice { get; set; } = null!;

    public int PartID { get; set; }
    [ForeignKey("PartID")] public Part Part { get; set; } = null!;

    [MaxLength(150)] public string PartNameSnapshot { get; set; } = string.Empty;

    public int Quantity { get; set; }
    [Column(TypeName = "numeric(10,2)")] public decimal UnitPrice { get; set; }
    [Column(TypeName = "numeric(12,2)")] public decimal LineTotal { get; set; }
}
