using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Models
{
    /// <summary>
    /// Represents a size option that can be applied across multiple products
    /// This is a master size table (e.g., S, M, L, XL, XXL, One Size)
    /// </summary>
    public class ProductSize
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SizeName { get; set; } // e.g., "S", "M", "L", "XL", "One Size"

        [StringLength(200)]
        public string? Description { get; set; } // e.g., "Small (Chest: 36-38 inches)"

        public int DisplayOrder { get; set; } = 0; // For sorting (S=1, M=2, L=3, etc.)

        public bool IsActive { get; set; } = true; // Admin can disable sizes

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        [ValidateNever]
        public ICollection<ProductColorSizeStock> ProductColorSizeStocks { get; set; } = new List<ProductColorSizeStock>();
    }
}
