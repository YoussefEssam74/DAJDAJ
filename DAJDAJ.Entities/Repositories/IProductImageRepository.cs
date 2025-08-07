using DAJDAJ.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Repositories
{
    public interface IProductImageRepository
    {
        IEnumerable<ProductImage> GetImagesByProductId(int productId);
    }
}
