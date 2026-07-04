using DAJDAJ.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Repositories
{
    public interface IProductColorStockRepository : IGenericRepository<ProductColorStock>
    {
        /// <summary>
        /// Gets stock information for a specific product and color
        /// </summary>
        ProductColorStock? GetStockByProductAndColor(int productId, string color);
        
        /// <summary>
        /// Gets all stock entries for a specific product
        /// </summary>
        IEnumerable<ProductColorStock> GetStockByProduct(int productId);
        
        /// <summary>
        /// Checks if a product/color combination is sold out
        /// </summary>
        bool IsSoldOut(int productId, string color);
        
        /// <summary>
        /// Checks if entire product is sold out (all colors)
        /// </summary>
        bool IsProductFullySoldOut(int productId);
        
        /// <summary>
        /// Gets available quantity for a specific product/color
        /// </summary>
        int GetAvailableQuantity(int productId, string color);
        
        /// <summary>
        /// Updates stock quantity for a product/color
        /// </summary>
        void UpdateStock(int productId, string color, int quantity);
        
        /// <summary>
        /// Decreases stock when order is placed
        /// </summary>
        bool DecreaseStock(int productId, string color, int quantity);
        
        /// <summary>
        /// Gets all low stock items (quantity <= 5)
        /// </summary>
        IEnumerable<ProductColorStock> GetLowStockItems();
        
        /// <summary>
        /// Gets colors available for a product (quantity > 0)
        /// </summary>
        IEnumerable<string> GetAvailableColors(int productId);
    }
}
