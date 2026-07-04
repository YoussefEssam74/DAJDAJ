using System;
using System.ComponentModel.DataAnnotations;

namespace DAJDAJ.Entities.Models
{
    // Only stores email requests for rate limiting - OTP stored in memory cache
    public class EmailOtp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // For rate limiting
        public string IpAddress { get; set; }
    }
}
