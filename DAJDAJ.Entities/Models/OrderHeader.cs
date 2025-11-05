using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Models
{
    public class OrderHeader
    {

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ValidateNever]

        public ApplicationUser ApplicationUser { get; set; }
        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ShippingDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        public string City { get; set; }
        public string Phone { get; set; }

        public string InstgramUserName { get; set; }




        [ValidateNever]
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }

}
