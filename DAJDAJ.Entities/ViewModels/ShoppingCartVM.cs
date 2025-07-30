using DAJDAJ.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.ViewModels
{
    public class ShoppingCartVM
    {
       public IEnumerable<Shoppingcart> CartsList { get; set; }
         public decimal TotalCarts { get; set; }
    }   
}
