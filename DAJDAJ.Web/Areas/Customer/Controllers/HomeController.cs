using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DAJDAJ.Entities.Models.Shoppingcart;
namespace DAJDAJ.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUntiOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; // Correct type for dependency injection

        public HomeController(IUntiOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment; // Assign the injected dependency
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();

            return View(products);
        }


        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id, "Category,ProductImages");
            if (product == null)
            {
                return NotFound();
            }
            List<string> sizes = new List<string>();
            List<string> colors = new List<string>();
            if (!string.IsNullOrWhiteSpace(product.Size))
            {
                sizes = product.Size
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Distinct()
                            .ToList();
            }
            if (!string.IsNullOrWhiteSpace(product.Color))
            {
                colors = product.Color
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => c.Trim())
                            .Distinct()
                            .ToList();
            }
            List<string> productImages = new();
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                productImages = product.ProductImages.Select(img => "/" + img.ImagePath.Replace("\\", "/")).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(product.Img))
            {
                productImages.Add("/" + product.Img.Replace("\\", "/"));
            }
            var shoppingcart = new Shoppingcart()
            {
                product = product,
                Sizes = sizes,
                Colors = colors,
                Count = 1,
                ProductImages = productImages
            };
            return View(shoppingcart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Shoppingcart shoppingcart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var cliam = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingcart.ApplicationUserId = cliam.Value;

            Shoppingcart Cartobj = _unitOfWork.ShoppingCart.GetFirstorDefault(
                u => u.ApplicationUserId == cliam.Value && u.ProductId == shoppingcart.ProductId
                );
            if (Cartobj != null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingcart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(Cartobj, shoppingcart.Count);
            }
                _unitOfWork.Complete();
            return RedirectToAction("Index");
        }


        public IActionResult Returns()
        {
            return View();
        }



    }
}