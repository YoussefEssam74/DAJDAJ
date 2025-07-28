using DAJDAJ.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.ViewModels
{
    public class Shoppingcart
    {
        public Product product { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> Colors { get; set; }
        [Range(1,100, ErrorMessage = "Count must be between 1 and 100.")]
        public int Count { get; set; }
        public List<string> ProductImages { get; set; } = new();

    }
}
