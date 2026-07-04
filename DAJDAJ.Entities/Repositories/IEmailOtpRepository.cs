using DAJDAJ.Entities.Models;
using System;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Repositories
{
    public interface IEmailOtpRepository : IGenericRepository<EmailOtp>
    {
        Task<EmailOtp> GetValidOtpByEmailAsync(string email);
        Task<int> GetOtpRequestCountAsync(string email, DateTime since);
        Task<int> GetOtpRequestCountByIpAsync(string ipAddress, DateTime since);
        Task CleanupExpiredOtpsAsync();
        Task<EmailOtp> GetOrCreateEmailRecordAsync(string email, string ipAddress);
    }
}
