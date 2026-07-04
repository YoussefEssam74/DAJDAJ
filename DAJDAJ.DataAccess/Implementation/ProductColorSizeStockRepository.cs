using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAJDAJ.DataAccess.Implementation
{
    public class ProductColorSizeStockRepository : GenericRepository<ProductColorSizeStock>, IProductColorSizeStockRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductColorSizeStockRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public ProductColorSizeStock? GetStockByProductColorSize(int productId, string color, string size)
        {
            return _context.ProductColorSizeStocks
                .FirstOrDefault(s => s.ProductId == productId && 
                    s.Color.ToLower() == color.ToLower() &&
                    s.Size.ToLower() == size.ToLower());
        }

        public IEnumerable<ProductColorSizeStock> GetStockByProduct(int productId)
        {
            return _context.ProductColorSizeStocks
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.Color)
                .ThenBy(s => s.Size)
                .ToList();
        }

        public IEnumerable<ProductColorSizeStock> GetStockByProductAndColor(int productId, string color)
        {
            return _context.ProductColorSizeStocks
                .Where(s => s.ProductId == productId && 
                    s.Color.ToLower() == color.ToLower())
                .OrderBy(s => s.Size)
                .ToList();
        }

        public bool IsSoldOut(int productId, string color, string size)
        {
            var stock = GetStockByProductColorSize(productId, color, size);
            return stock == null || stock.Quantity == 0;
        }

        public bool IsProductFullySoldOut(int productId)
        {
            var stocks = GetStockByProduct(productId);
            return stocks.Any() && stocks.All(s => s.Quantity == 0);
        }

        public int GetAvailableQuantity(int productId, string color, string size)
        {
            var stock = GetStockByProductColorSize(productId, color, size);
            return stock?.Quantity ?? 0;
        }

        public void UpdateStock(int productId, string color, string size, int quantity)
        {
            var stock = GetStockByProductColorSize(productId, color, size);
            
            if (stock != null)
            {
                stock.Quantity = quantity;
                stock.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                // Create new stock entry
                _context.ProductColorSizeStocks.Add(new ProductColorSizeStock
                {
                    ProductId = productId,
                    Color = color,
                    Size = size,
                    Quantity = quantity,
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        public bool DecreaseStock(int productId, string color, string size, int quantity)
        {
            var stock = GetStockByProductColorSize(productId, color, size);
            
            if (stock == null || stock.Quantity < quantity)
            {
                return false; // Not enough stock
            }
            
            stock.Quantity -= quantity;
            stock.LastUpdated = DateTime.UtcNow;
            return true;
        }

        public IEnumerable<ProductColorSizeStock> GetLowStockItems()
        {
            return _context.ProductColorSizeStocks
                .Include(s => s.Product)
                .Where(s => s.Quantity > 0 && s.Quantity <= 5)
                .OrderBy(s => s.Quantity)
                .ToList();
        }

        public IEnumerable<string> GetAvailableColors(int productId)
        {
            return _context.ProductColorSizeStocks
                .Where(s => s.ProductId == productId && s.Quantity > 0)
                .Select(s => s.Color)
                .Distinct()
                .ToList();
        }

        public IEnumerable<string> GetAvailableSizes(int productId, string color)
        {
            return _context.ProductColorSizeStocks
                .Where(s => s.ProductId == productId && 
                    s.Color.ToLower() == color.ToLower() && 
                    s.Quantity > 0)
                .Select(s => s.Size)
                .Distinct()
                .ToList();
        }
    }
}
