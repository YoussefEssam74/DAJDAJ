using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAJDAJ.DataAccess.Implementation
{
    public class EmailOtpRepository : GenericRepository<EmailOtp>, IEmailOtpRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailOtpRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EmailOtp> GetValidOtpByEmailAsync(string email)
        {
            // OTP is now stored in cache, not in database
            // This method is kept for backward compatibility but returns null
            return await Task.FromResult<EmailOtp>(null);
        }

        public async Task<int> GetOtpRequestCountAsync(string email, DateTime since)
        {
            return await _context.Set<EmailOtp>()
                .Where(o => o.Email == email && o.CreatedAt >= since)
                .CountAsync();
        }

        public async Task<int> GetOtpRequestCountByIpAsync(string ipAddress, DateTime since)
        {
            return await _context.Set<EmailOtp>()
                .Where(o => o.IpAddress == ipAddress && o.CreatedAt >= since)
                .CountAsync();
        }

        public async Task CleanupExpiredOtpsAsync()
        {
            var expiredDate = DateTime.UtcNow.AddDays(-1);
            var expiredOtps = await _context.Set<EmailOtp>()
                .Where(o => o.CreatedAt < expiredDate)
                .ToListAsync();

            _context.Set<EmailOtp>().RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailOtp> GetOrCreateEmailRecordAsync(string email, string ipAddress)
        {
            // First check if email already exists
            var existing = await _context.Set<EmailOtp>()
                .FirstOrDefaultAsync(e => e.Email == email);

            if (existing != null)
            {
                // Update timestamp and IP for existing record
                existing.IpAddress = ipAddress;
                existing.CreatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existing;
            }

            // Create new record
            var newRecord = new EmailOtp
            {
                Email = email,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Set<EmailOtp>().Add(newRecord);
                await _context.SaveChangesAsync();
                return newRecord;
            }
            catch (DbUpdateException ex)
            {
                // Handle race condition - another thread inserted same email
                var innerException = ex.InnerException?.Message ?? ex.Message;
                if (innerException.Contains("unique", StringComparison.OrdinalIgnoreCase) ||
                    innerException.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    // Detach failed entity and fetch the existing one
                    _context.Entry(newRecord).State = EntityState.Detached;
                    var existingRecord = await _context.Set<EmailOtp>()
                        .FirstOrDefaultAsync(e => e.Email == email);
                    
                    if (existingRecord != null)
                    {
                        existingRecord.IpAddress = ipAddress;
                        existingRecord.CreatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return existingRecord;
                    }
                }
                throw;
            }
        }
    }
}
