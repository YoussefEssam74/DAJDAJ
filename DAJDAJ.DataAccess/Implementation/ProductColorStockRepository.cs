using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.DataAccess.Implementation
{
    public class ProductColorStockRepository : GenericRepository<ProductColorStock>, IProductColorStockRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductColorStockRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public ProductColorStock? GetStockByProductAndColor(int productId, string color)
        {
            return _context.ProductColorStocks
                .FirstOrDefault(s => s.ProductId == productId && 
                    s.Color.ToLower() == color.ToLower());
        }

        public IEnumerable<ProductColorStock> GetStockByProduct(int productId)
        {
            return _context.ProductColorStocks
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.Color)
                .ToList();
        }

        public bool IsSoldOut(int productId, string color)
        {
            var stock = GetStockByProductAndColor(productId, color);
            return stock == null || stock.Quantity == 0;
        }

        public bool IsProductFullySoldOut(int productId)
        {
            var stocks = GetStockByProduct(productId);
            return stocks.Any() && stocks.All(s => s.Quantity == 0);
        }

        public int GetAvailableQuantity(int productId, string color)
        {
            var stock = GetStockByProductAndColor(productId, color);
            return stock?.Quantity ?? 0;
        }

        public void UpdateStock(int productId, string color, int quantity)
        {
            var stock = GetStockByProductAndColor(productId, color);
            
            if (stock != null)
            {
                stock.Quantity = quantity;
                stock.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                // Create new stock entry
                _context.ProductColorStocks.Add(new ProductColorStock
                {
                    ProductId = productId,
                    Color = color,
                    Quantity = quantity,
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        public bool DecreaseStock(int productId, string color, int quantity)
        {
            var stock = GetStockByProductAndColor(productId, color);
            
            if (stock == null || stock.Quantity < quantity)
            {
                return false; // Not enough stock
            }
            
            stock.Quantity -= quantity;
            stock.LastUpdated = DateTime.UtcNow;
            return true;
        }

        public IEnumerable<ProductColorStock> GetLowStockItems()
        {
            return _context.ProductColorStocks
                .Include(s => s.Product)
                .Where(s => s.Quantity > 0 && s.Quantity <= 5)
                .OrderBy(s => s.Quantity)
                .ToList();
        }

        public IEnumerable<string> GetAvailableColors(int productId)
        {
            return _context.ProductColorStocks
                .Where(s => s.ProductId == productId && s.Quantity > 0)
                .Select(s => s.Color)
                .Distinct()
                .ToList();
        }
    }
}
