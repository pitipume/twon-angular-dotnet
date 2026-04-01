using Resend;
using Twon.Application.Common.Interfaces;

namespace Twon.Infrastructure.Services;

public class ResendEmailService(IResend resend) : IEmailService
{
    public async Task SendOtpEmailAsync(string email, string displayName, string otp)
    {
        var message = new EmailMessage
        {
            From = "Twon <noreply@twon.app>",
            To = [email],
            Subject = "Your Twon verification code",
            HtmlBody = $"""
                <div style="font-family:sans-serif;max-width:400px">
                    <h2>Hi {displayName},</h2>
                    <p>Your verification code is:</p>
                    <h1 style="letter-spacing:8px;font-size:48px">{otp}</h1>
                    <p>This code expires in 5 minutes. Do not share it with anyone.</p>
                </div>
            """
        };

        await resend.EmailSendAsync(message);
    }
}
