using DAJDAJ.DataAccess;
using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ("Admin"))]

    public class OrderController : Controller
    {
        private readonly IUntiOfWork _untiOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUntiOfWork untiOfWork)
        {
            _untiOfWork = untiOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult GetData()
        {
            // Test without includes first
            var orders = _untiOfWork.OrderHeader.GetAll()
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Address,
                    o.City,
                    o.Phone,
                    o.InstgramUserName,
                    o.OrderDate,
                    o.TotalPrice,
                    o.OrderStatus,
                    o.PaymentStatus,
                    o.TrackingNumber,
                    o.Carrier,
                    o.PaymentMethod,
                }).ToList();

            return Json(new { data = orders });
        }

        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == orderid),
                OrderDetails = _untiOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == orderid, "Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.Phone = OrderVM.OrderHeader.Phone;
            orderFromDb.Address = OrderVM.OrderHeader.Address;
            orderFromDb.City = OrderVM.OrderHeader.City;
            orderFromDb.InstgramUserName = OrderVM.OrderHeader.InstgramUserName;

            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _untiOfWork.OrderHeader.Update(orderFromDb);
            _untiOfWork.Complete();

            TempData["Update"] = "Item has been updated successfully";

            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess(OrderVM OrderVM)
        {
            _untiOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
            _untiOfWork.Complete();

            TempData["Update"] = "Order status has been updated to 'Processing' successfully.";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);


            orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderFromDb.OrderStatus = SD.Shipped;
            orderFromDb.ShippingDate = DateTime.Now;

            _untiOfWork.OrderHeader.Update(orderFromDb);
            _untiOfWork.Complete();

            TempData["Update"] = "Order has been shipped successfully.";

            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderFromDb.PaymentMethod == "CashOnDelivery")
            {
                _untiOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, null);
            }

            _untiOfWork.Complete();


            TempData["Update"] = "Order has been Cancelled successfully.";

            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

    }
}