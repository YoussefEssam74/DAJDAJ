using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAJDAJ.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace DAJDAJ.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUntiOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMemoryCache _cache;

        // Rate limiting configuration
        private const int MaxOtpRequestsPerEmail = 3; // per time window
        private const int MaxOtpRequestsPerIp = 10; // per time window
        private const int RateLimitWindowMinutes = 15;
        private const int OtpExpirationMinutes = 15;
        private const int MaxOtpAttempts = 5;
        private const int OtpCooldownSeconds = 60; // 1 minute cooldown

        public AuthController(
            IUntiOfWork unitOfWork,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _cache = cache;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpResponse
                {
                    Success = false,
                    Message = "Invalid email format"
                });
            }

            try
            {
                var email = request.Email.ToLower().Trim();
                var ipAddress = GetClientIpAddress();

                // Check cooldown using cache
                var cooldownKey = $"otp:cooldown:{email}";
                if (_cache.TryGetValue<DateTime>(cooldownKey, out var lastRequest))
                {
                    var secondsRemaining = (int)(OtpCooldownSeconds - (DateTime.UtcNow - lastRequest).TotalSeconds);
                    if (secondsRemaining > 0)
                    {
                        return BadRequest(new OtpResponse
                        {
                            Success = false,
                            Message = $"Please wait {secondsRemaining} seconds before requesting another OTP"
                        });
                    }
                }

                // Rate limiting by email
                var emailRequestCount = await _unitOfWork.EmailOtp.GetOtpRequestCountAsync(
                    email,
                    DateTime.UtcNow.AddMinutes(-RateLimitWindowMinutes)
                );

                if (emailRequestCount >= MaxOtpRequestsPerEmail)
                {
                    return BadRequest(new OtpResponse
                    {
                        Success = false,
                        Message = "Too many OTP requests. Please try again later."
                    });
                }

                // Rate limiting by IP
                var ipRequestCount = await _unitOfWork.EmailOtp.GetOtpRequestCountByIpAsync(
                    ipAddress,
                    DateTime.UtcNow.AddMinutes(-RateLimitWindowMinutes)
                );

                if (ipRequestCount >= MaxOtpRequestsPerIp)
                {
                    return BadRequest(new OtpResponse
                    {
                        Success = false,
                        Message = "Too many requests from this location. Please try again later."
                    });
                }

                // Generate OTP
                var otp = OtpHelper.GenerateOtp();
                var hashedOtp = OtpHelper.HashOtp(otp);

                // Store OTP in cache (NOT in database!)
                var cacheKey = $"otp:{email}";
                var cacheData = new OtpCacheData
                {
                    HashedOtp = hashedOtp,
                    FailedAttempts = 0,
                    CreatedAt = DateTime.UtcNow
                };
                _cache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(OtpExpirationMinutes)
                });

                // Set cooldown
                _cache.Set(cooldownKey, DateTime.UtcNow, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(OtpCooldownSeconds)
                });

                // Get or create email record (NO DUPLICATES!)
                await _unitOfWork.EmailOtp.GetOrCreateEmailRecordAsync(email, ipAddress);
                _unitOfWork.Complete();

                // Send OTP via email
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Your Login Code</h2>
                            <p>Hello,</p>
                            <p>Your one-time password (OTP) for login is:</p>
                            <div style='background-color: #f4f4f4; padding: 20px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #007bff; letter-spacing: 5px; margin: 0;'>{otp}</h1>
                            </div>
                            <p>This code will expire in {OtpExpirationMinutes} minutes.</p>
                            <p>If you didn't request this code, please ignore this email.</p>
                            <hr style='border: 1px solid #eee; margin: 20px 0;' />
                            <p style='color: #666; font-size: 12px;'>DAJDAJ - Secure Login</p>
                        </div>
                    </body>
                    </html>";

                await _emailSender.SendEmailAsync(email, "Your Login Code", emailBody);

                // Cleanup old OTPs (background task)
                _ = Task.Run(async () => await _unitOfWork.EmailOtp.CleanupExpiredOtpsAsync());

                return Ok(new OtpResponse
                {
                    Success = true,
                    Message = "OTP sent successfully to your email",
                    Data = new { email = MaskEmail(email) }
                });
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, new OtpResponse
                {
                    Success = false,
                    Message = "An error occurred while processing your request"
                });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpResponse
                {
                    Success = false,
                    Message = "Invalid request"
                });
            }

            try
            {
                var email = request.Email.ToLower().Trim();
                var otp = request.Otp;
                var returnUrl = request.ReturnUrl;

                // Get OTP from memory cache
                var cacheKey = $"otp:{email}";
                if (!_cache.TryGetValue<OtpCacheData>(cacheKey, out var otpData))
                {
                    return BadRequest(new OtpResponse
                    {
                        Success = false,
                        Message = "Invalid or expired OTP"
                    });
                }

                // Check if OTP has exceeded max attempts
                if (otpData.FailedAttempts >= MaxOtpAttempts)
                {
                    return BadRequest(new OtpResponse
                    {
                        Success = false,
                        Message = "OTP has been locked due to too many failed attempts"
                    });
                }

                // Verify OTP
                if (!OtpHelper.VerifyOtp(otp, otpData.HashedOtp))
                {
                    // Increment failed attempts in cache
                    otpData.FailedAttempts++;
                    _cache.Set(cacheKey, otpData, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(OtpExpirationMinutes)
                    });

                    var remainingAttempts = MaxOtpAttempts - otpData.FailedAttempts;
                    return BadRequest(new OtpResponse
                    {
                        Success = false,
                        Message = remainingAttempts > 0
                            ? $"Invalid OTP. {remainingAttempts} attempt(s) remaining"
                            : "Invalid OTP. Maximum attempts exceeded"
                    });
                }

                // Remove OTP from cache (mark as used)
                _cache.Remove(cacheKey);

                // Check if user exists
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // Auto-create user with email-only account (optimized for speed)
                    user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true, // Auto-confirm since they verified via OTP
                        LockoutEnabled = false // Disable lockout for OTP-only accounts
                    };

                    // Create user without password (faster than with password hashing)
                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        return StatusCode(500, new OtpResponse
                        {
                            Success = false,
                            Message = "Failed to create user account"
                        });
                    }
                }

                // Sign in the user (with persistent cookie for better UX)
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Determine redirect URL
                string redirectUrl;
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    redirectUrl = returnUrl;
                }
                else
                {
                    redirectUrl = Url.Action("Index", "Home", new { area = "Customer" });
                }

                return Ok(new OtpResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new
                    {
                        email = user.Email,
                        redirectUrl = redirectUrl
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, new OtpResponse
                {
                    Success = false,
                    Message = "An error occurred while processing your request"
                });
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] SendOtpRequest request)
        {
            // Reuse the send-otp logic with same rate limiting
            return await SendOtp(request);
        }

        // Helper methods
        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            // Check for forwarded IP (if behind proxy)
            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return ipAddress ?? "Unknown";
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var localPart = parts[0];
            var domain = parts[1];

            if (localPart.Length <= 2)
                return $"{localPart[0]}***@{domain}";

            return $"{localPart[0]}***{localPart[^1]}@{domain}";
        }
    }

    // Cache data structure for OTP storage in memory
    public class OtpCacheData
    {
        public string HashedOtp { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
