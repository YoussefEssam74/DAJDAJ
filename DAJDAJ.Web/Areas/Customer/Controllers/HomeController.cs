using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using static DAJDAJ.Entities.ViewModels.Shoppingcart;
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
            var product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id, "Category");

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

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string productFolder = Path.Combine(wwwRootPath, "Images", "Products", product.Id.ToString());

            List<string> productImages = new();
            if (Directory.Exists(productFolder))
            {
                var files = Directory.GetFiles(productFolder);
                productImages = files.Select(file =>
                    Path.Combine("/Images/Products", product.Id.ToString(), Path.GetFileName(file)).Replace("\\", "/")
                ).ToList();
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

        public IActionResult Returns()
        {
            return View();
        }



    }
}