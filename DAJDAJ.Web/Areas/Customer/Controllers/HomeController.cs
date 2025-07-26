using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using static DAJDAJ.Entities.ViewModels.Shoppingcart;
using Microsoft.AspNetCore.Mvc;
namespace DAJDAJ.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUntiOfWork _unitOfWork;

        public HomeController(IUntiOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id, "Category");

            if (product == null)
            {
                return NotFound();
            }

            // هنا هنفك المقاسات والألوان لو موجودة
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

            var shoppingcart = new Shoppingcart()
            {
                product = product,
                Sizes = sizes,
                Colors = colors,
                Count = 1
            };

            return View(shoppingcart);
        }

    }
}