using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Models
{
    public class OrderForShippingDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItemDto> Items { get; set; }

    }
}
