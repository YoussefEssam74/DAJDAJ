using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Models
{
    public class ProductColorStock
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public Product Product { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Color { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Helper property to check if this color is sold out
        public bool IsSoldOut => Quantity == 0;
        
        // Helper property to check if stock is low
        public bool IsLowStock => Quantity > 0 && Quantity <= 5;
    }
}
