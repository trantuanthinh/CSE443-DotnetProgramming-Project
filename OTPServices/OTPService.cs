using System.Collections.Concurrent;
using Project.MailServices;

namespace Project.OTPServices
{
    public class OtpService(MailService mailService)
    {
        private readonly MailService _mailService = mailService;
        private readonly double _expiryMinute = 5;
        private static readonly ConcurrentDictionary<
            string,
            (string Otp, DateTime Expiry)
        > OtpStorage = new();

        private string GenerateOtp()
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }

        private string GenerateBody(string otp)
        {
            return $@"
        <!DOCTYPE html>
            <html lang=""en"">
            <body style=""font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 20px;"">
                <div style=""background-color: #ffffff; padding: 20px 30px; max-width: 400px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); text-align: center; margin: 0 auto;"">
                    <h1 style=""color: #333; font-size: 24px; margin-bottom: 20px;"">OTP Verification</h1>
                    <p style=""font-size: 16px; color: #555; margin-bottom: 10px;"">
                        Your OTP code is 
                        <strong style=""color: #007bff; font-size: 18px;"">{otp}</strong>
                    </p>
                    <p style=""font-size: 14px; color: #888; margin-bottom: 20px;"">
                        It will expire in {_expiryMinute} minutes.
                    </p>
                    <p style=""font-size: 14px; color: #ff0000; font-weight: bold;"">
                        Please do not share this code with anyone for security reasons.
                    </p>
                </div>
            </body>
            </html>";
        }

        public async Task<bool> SendOTP(string email)
        {
            // Remove any existing OTP entry for the email
            OtpStorage.TryRemove(email, out _);

            // Generate a new OTP and set expiry
            string otp = GenerateOtp();
            DateTime expiry = DateTime.UtcNow.AddMinutes(_expiryMinute);
            OtpStorage.TryAdd(email, (otp, expiry));

            string body = GenerateBody(otp);

            return await _mailService.SendMail(email, "OTP Verification", body);
        }

        public async Task<bool> VerifyOTP(string email, string inputOTP)
        {
            if (OtpStorage.TryGetValue(email, out var stored) && stored.Expiry > DateTime.UtcNow)
            {
                if (stored.Otp == inputOTP)
                {
                    OtpStorage.TryRemove(email, out _); // Remove OTP after successful verification
                    return true;
                }
                else
                {
                    OtpStorage.TryRemove(email, out _); // Remove OTP if verification fails
                }
            }
            return false;
        }
    }
}
