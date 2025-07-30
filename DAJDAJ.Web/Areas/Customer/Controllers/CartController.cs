using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAJDAJ.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUntiOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public int TotalCarts { get; set; }
        public CartController(IUntiOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM
            {
                CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cliam.Value, "Product")
            };

            foreach (var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.product.Price);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.Id == cartid);
            _unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.Id == cartid);
            if(shoppingcart.Count<1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingcart);
                _unitOfWork.Complete();
                return RedirectToAction("Index","Home");
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
            }
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.Id == cartid);
            _unitOfWork.ShoppingCart.Remove(shoppingcart);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}
