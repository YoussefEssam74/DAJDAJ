using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.Entities.Repositories
{
    public interface IUntiOfWork : IDisposable
    {
        ICategoryRepository Category{ get; }
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; } 
        IOrderDetailsRepository OrderDetails { get; }
        
        IProductImageRepository ProductImage { get; }
        IApplicationUserRepository ApplicationUser { get; }
        int Complete();
    }
}
