using System;

namespace DAJDAJ.Entities.ViewModels
{
    public class OrderFilterVM
    {
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
    }
}
