using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.DataAccess.Implementation
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ProductImage> GetImagesByProductId(int productId)
        {
            return _context.ProductImages.Where(p => p.ProductId == productId).ToList();
        }
    }

}
