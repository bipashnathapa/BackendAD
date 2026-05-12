using System.ComponentModel.DataAnnotations;

namespace Vehicle.Application.DTOs;

public class PartDTO
{
    public int PartID { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartCode { get; set; }
    public string? Compatibility { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int? VendorID { get; set; }
    public string? VendorName { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePartDTO
{
    [Required, MaxLength(150)] public string PartName { get; set; } = string.Empty;
    [MaxLength(80)] public string? PartCode { get; set; }
    [MaxLength(120)] public string? Compatibility { get; set; }
    [Range(0, 999999)] public decimal UnitPrice { get; set; }
    [Range(0, 100000)] public int StockQuantity { get; set; }
    [Range(0, 1000)] public int LowStockThreshold { get; set; } = 10;
    public int? VendorID { get; set; }
}

public class CreateSaleInvoiceDTO
{
    [Required] public int CustomerID { get; set; }
    [Required, MinLength(1)] public List<SaleLineDTO> Items { get; set; } = new();
    [Range(0, 100)] public decimal DiscountPercent { get; set; }
    [Range(0, 100)] public decimal TaxPercent { get; set; } = 13; // NPR VAT
    [Range(0, 9999999)] public decimal AmountPaid { get; set; }
    [MaxLength(30)] public string? PaymentMethod { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}

public class SaleLineDTO
{
    [Required] public int PartID { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; }
}

public class SaleInvoiceDTO
{
    public int InvoiceID { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime InvoiceDate { get; set; }
    public List<SaleInvoiceItemDTO> Items { get; set; } = new();
}

public class SaleInvoiceItemDTO
{
    public int PartID { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
