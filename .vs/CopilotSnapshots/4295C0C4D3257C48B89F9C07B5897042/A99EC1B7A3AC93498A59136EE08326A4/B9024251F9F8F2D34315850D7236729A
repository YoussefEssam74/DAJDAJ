using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.DataAccess.Implementation
{
    public class ProductRepository:GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) :base(context) 
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductInDb = _context.products.FirstOrDefault(x => x.Id == product.Id);
            if (ProductInDb != null)
            {
                ProductInDb.Name = product.Name;
                ProductInDb.Description = product.Description;
              
                ProductInDb.Price = product.Price;
                ProductInDb.Size = product.Size;
                ProductInDb.Color = product.Color;
                ProductInDb.Img = product.Img;
                ProductInDb.IsOneSize = product.IsOneSize;
                ProductInDb.CategoryId = product.CategoryId;
            }
        }
    }
}
