using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interface.IServices;

public interface ISaleInvoiceService
{
    Task<SaleInvoiceDTO?> CreateAsync(string staffUserId, CreateSaleInvoiceDTO dto);
    Task<SaleInvoiceDTO?> GetByIdAsync(int id);
}
