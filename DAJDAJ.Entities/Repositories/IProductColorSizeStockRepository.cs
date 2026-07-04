using DAJDAJ.Entities.Models;
using System.Collections.Generic;

namespace DAJDAJ.Entities.Repositories
{
    public interface IProductColorSizeStockRepository : IGenericRepository<ProductColorSizeStock>
    {
        /// <summary>
        /// Gets stock information for a specific product, color, and size
        /// </summary>
        ProductColorSizeStock? GetStockByProductColorSize(int productId, string color, string size);
        
        /// <summary>
        /// Gets all stock entries for a specific product
        /// </summary>
        IEnumerable<ProductColorSizeStock> GetStockByProduct(int productId);
        
        /// <summary>
        /// Gets all stock entries for a specific product and color
        /// </summary>
        IEnumerable<ProductColorSizeStock> GetStockByProductAndColor(int productId, string color);
        
        /// <summary>
        /// Checks if a product/color/size combination is sold out
        /// </summary>
        bool IsSoldOut(int productId, string color, string size);
        
        /// <summary>
        /// Checks if entire product is sold out (all color+size combinations)
        /// </summary>
        bool IsProductFullySoldOut(int productId);
        
        /// <summary>
        /// Gets available quantity for a specific product/color/size
        /// </summary>
        int GetAvailableQuantity(int productId, string color, string size);
        
        /// <summary>
        /// Updates stock quantity for a product/color/size
        /// </summary>
        void UpdateStock(int productId, string color, string size, int quantity);
        
        /// <summary>
        /// Decreases stock when order is placed
        /// </summary>
        bool DecreaseStock(int productId, string color, string size, int quantity);
        
        /// <summary>
        /// Gets all low stock items (quantity <= 5)
        /// </summary>
        IEnumerable<ProductColorSizeStock> GetLowStockItems();
        
        /// <summary>
        /// Gets available colors for a product (with stock > 0)
        /// </summary>
        IEnumerable<string> GetAvailableColors(int productId);
        
        /// <summary>
        /// Gets available sizes for a product and color (with stock > 0)
        /// </summary>
        IEnumerable<string> GetAvailableSizes(int productId, string color);
    }
}
