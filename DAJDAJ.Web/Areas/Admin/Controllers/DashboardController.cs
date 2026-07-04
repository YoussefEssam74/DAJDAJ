using DAJDAJ.DataAccess;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ("Admin"))]
    public class DashboardController : Controller
    {
        private readonly IUntiOfWork _untiOfWork;
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(IUntiOfWork untiOfWork, ApplicationDbContext dbContext)
        {
            _untiOfWork = untiOfWork;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewBag.Orders = _untiOfWork.OrderHeader.GetAll().Count();
            ViewBag.EarnedOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Earned).Count();
            ViewBag.BookedOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Booked).Count();
            ViewBag.ReturnedOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Return).Count();
            ViewBag.CancelledOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Cancelled).Count();
            
            // Count distinct/unique emails from EmailOtp table
            ViewBag.Users = _untiOfWork.EmailOtp.GetAll()
                .Select(x => x.Email)
                .Distinct()
                .Count();
            
            ViewBag.Products = _untiOfWork.Product.GetAll().Count();
            
            // Get total stock from database directly
            var totalStock = _dbContext.ProductColorSizeStocks.Sum(x => (int?)x.Quantity) ?? 0;
            ViewBag.TotalStock = totalStock;

           

            return View();
        }
    }
}
