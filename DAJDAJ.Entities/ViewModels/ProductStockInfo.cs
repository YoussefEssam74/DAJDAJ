namespace DAJDAJ.Entities.ViewModels
{
    public class ProductStockInfo
    {
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        
        public bool IsSoldOut => Quantity == 0;
        public bool IsLowStock => Quantity > 0 && Quantity <= 5;
    }
}
