using System;
using System.Security.Cryptography;
using System.Text;

namespace DAJDAJ.Utilities
{
    public static class OtpHelper
    {
        /// <summary>
        /// Generate a 6-digit OTP
        /// </summary>
        public static string GenerateOtp()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);
                int value = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
                return (value % 1000000).ToString("D6");
            }
        }

        /// <summary>
        /// Hash OTP using SHA256
        /// </summary>
        public static string HashOtp(string otp)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verify OTP against hashed value
        /// </summary>
        public static bool VerifyOtp(string otp, string hashedOtp)
        {
            string hashOfInput = HashOtp(otp);
            return hashOfInput.Equals(hashedOtp, StringComparison.OrdinalIgnoreCase);
        }
    }
}
