namespace Vehicle.Application.Interface.IServices;

public interface IEmailOtpService
{
    Task SendOtpAsync(string email, string purpose, string displayName, CancellationToken cancellationToken = default);
    bool VerifyOtp(string email, string purpose, string otpCode);
    void ClearOtp(string email, string purpose);
}
