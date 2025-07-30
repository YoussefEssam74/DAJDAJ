using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        [Required(ErrorMessage = "The Description field is required.")]

        public string Description { get; set; }
        [DisplayName("Image")]
        [ValidateNever] // لا تحقق هنا
        public string Img { get; set; } 
        [Required]

        public decimal Price { get; set; }
        [Required]

        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]

        public Category Category { get; set; }
        public string Color { get; set; }

        public string Size { get; set; } // E.g., "S", "M", "L", "XL", or "One Size"

        public bool IsOneSize { get; set; } // True if product comes in only one size

        [ValidateNever]
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}