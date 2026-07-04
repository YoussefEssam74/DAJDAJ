using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAJDAJ.Entities.Models
{
    /// <summary>
    /// Manages stock for each Product Color + Size combination
    /// </summary>
    public class ProductColorSizeStock
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        
        public Product Product { get; set; }

        [Required]
        [StringLength(50)]
        public string Color { get; set; }

        [Required]
        [StringLength(50)]
        public string Size { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Helper Properties
        [NotMapped]
        public bool IsSoldOut => Quantity == 0;

        [NotMapped]
        public bool IsLowStock => Quantity > 0 && Quantity <= 5;
    }
}
