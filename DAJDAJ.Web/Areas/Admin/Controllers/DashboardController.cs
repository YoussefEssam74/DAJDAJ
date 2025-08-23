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
            ViewBag.ShippedOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Shipped).Count();
            ViewBag.CancelledOrders = _untiOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Cancelled).Count();
            ViewBag.Users = _untiOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Products = _untiOfWork.Product.GetAll().Count();

            var googleUsersCount = _dbContext.UserLogins
                .Where(l => l.LoginProvider == "Google")
                .Select(l => l.UserId)
                .Distinct()
                .Count();

            ViewBag.GoogleUsers = googleUsersCount;

            return View();
        }
    }
}
