using DAJDAJ.Entities.Repositories;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
        [Area("Admin")]
        [Authorize(Roles = ("Admin"))]
    public class DashboardController : Controller
    {
        private IUntiOfWork _untiOfWork;
        public DashboardController(IUntiOfWork untiOfWork)
        {
            _untiOfWork = untiOfWork;
        }
        public IActionResult Index()
        {
            ViewBag.Orders=_untiOfWork.OrderHeader.GetAll().Count();
            ViewBag.ShippedOrders=_untiOfWork.OrderHeader.GetAll(x=>x.OrderStatus==SD.Shipped).Count();
            ViewBag.CancelledOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Cancelled).Count();
            ViewBag.Users = _untiOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Products=_untiOfWork.Product.GetAll().Count();

            return View();
        }
    }
}
