namespace Twon.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string email, string displayName, string otp);
}
