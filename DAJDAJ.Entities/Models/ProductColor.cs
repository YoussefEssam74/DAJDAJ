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
    /// Represents a specific color variant of a product
    /// Each product can have multiple colors
    /// </summary>
    public class ProductColor
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }

        [Required]
        [StringLength(50)]
        public string ColorName { get; set; } // e.g., "Black", "White", "Red"

        [StringLength(20)]
        public string? ColorCode { get; set; } // e.g., "#000000" for UI display (optional)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ValidateNever]
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        [ValidateNever]
        public ICollection<ProductColorSizeStock> ProductColorSizeStocks { get; set; } = new List<ProductColorSizeStock>();

        // Computed property: Total stock across all sizes for this color
        public int TotalStock => ProductColorSizeStocks?.Sum(s => s.Quantity) ?? 0;

        // Check if this color is completely sold out (all sizes)
        public bool IsFullySoldOut => TotalStock == 0;
    }
}
