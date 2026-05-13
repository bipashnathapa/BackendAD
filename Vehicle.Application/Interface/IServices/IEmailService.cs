namespace Vehicle.Application.Interface.IServices;

public interface IEmailService
{
    Task<bool> SendAsync(string to, string subject, string body);
}
