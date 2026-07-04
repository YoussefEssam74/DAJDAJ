using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList.Extensions;
using static DAJDAJ.Entities.Models.Shoppingcart;

namespace DAJDAJ.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUntiOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUntiOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int ? page)
        {
            var PageNumber = page ?? 1;
            var PageSize = 6;
            var products = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);

            // Add stock information for each product
            var productStockData = new Dictionary<int, bool>();
            foreach (var product in products)
            {
                var isFullySoldOut = _unitOfWork.ProductColorSizeStock.IsProductFullySoldOut(product.Id);
                productStockData[product.Id] = isFullySoldOut;
            }
            ViewBag.ProductStockData = productStockData;

            // Set cart count for layout
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    ViewBag.CartCount = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Sum(x => x.Count);
                }
                else
                {
                    ViewBag.CartCount = 0;
                }
            }
            else
            {
                ViewBag.CartCount = 0;
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id, Includeword: "Category,ProductImages");
            if (product == null)
            {
                return NotFound();
            }

            var colors = new List<string>();
            var sizes = new List<string>();

            // Collect unique colors from ProductImages
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                colors = product.ProductImages
                    .Where(img => !string.IsNullOrWhiteSpace(img.Color))
                    .Select(img => img.Color.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            // Extract sizes from Product.Size as is
            if (!string.IsNullOrWhiteSpace(product.Size))
            {
                sizes = product.Size.Split(',').Select(s => s.Trim()).ToList();
            }

            // Collect images from ProductImages and Img
            var productImages = new List<string>();
            var imageColorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (product.ProductImages != null && product.ProductImages.Any())
            {
                foreach (var img in product.ProductImages)
                {
                    var imagePath = "/" + img.ImagePath.Replace("\\", "/");
                    productImages.Add(imagePath);

                    // Map image to color - keep original color casing
                    if (!string.IsNullOrEmpty(img.Color))
                    {
                        var colorKey = img.Color.Trim();
                        imageColorMap[colorKey] = imagePath;
                        System.Diagnostics.Debug.WriteLine($"Mapping color '{colorKey}' to image '{imagePath}'");
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(product.Img))
            {
                productImages.Add("/" + product.Img.Replace("\\", "/"));
            }

            // Get stock information for each color+size combination
            var stockInfo = new List<ProductStockInfo>();
            var isProductFullySoldOut = false;

            if (colors.Any() && sizes.Any())
            {
                foreach (var color in colors)
                {
                    foreach (var size in sizes)
                    {
                        // Check if stock record exists, if not create one with quantity 0
                        var existingStock = _unitOfWork.ProductColorSizeStock.GetStockByProductColorSize(id, color, size);
                        if (existingStock == null)
                        {
                            _unitOfWork.ProductColorSizeStock.UpdateStock(id, color, size, 0);
                            _unitOfWork.Complete();
                        }

                        var quantity = _unitOfWork.ProductColorSizeStock.GetAvailableQuantity(id, color, size);
                        stockInfo.Add(new ProductStockInfo
                        {
                            Color = color,
                            Size = size,
                            Quantity = quantity
                        });
                    }
                }

                isProductFullySoldOut = _unitOfWork.ProductColorSizeStock.IsProductFullySoldOut(id);
            }

            var shoppingcart = new Shoppingcart()
            {
                product = product,
                ProductId = id,
                Sizes = sizes,
                Colors = colors,
                Count = 1,
                ProductImages = productImages
            };

            // Add color-image map and stock info to ViewBag
            ViewBag.ImageColorMap = imageColorMap;
            ViewBag.StockInfo = stockInfo;
            ViewBag.IsProductFullySoldOut = isProductFullySoldOut;
            ViewBag.Viewers = new Random().Next(37, 54);

            // Log for debugging
            System.Diagnostics.Debug.WriteLine($"Product Colors from ProductImages: {string.Join(", ", colors)}");
            System.Diagnostics.Debug.WriteLine($"ImageColorMap Count: {imageColorMap.Count}");
            foreach (var kvp in imageColorMap)
            {
                System.Diagnostics.Debug.WriteLine($"Color '{kvp.Key}' -> Image '{kvp.Value}'");
            }

            return View(shoppingcart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Shoppingcart model)
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validate stock availability by Color AND Size
            if (!string.IsNullOrEmpty(model.SelectedColor) && !string.IsNullOrEmpty(model.SelectedSize))
            {
                var availableStock = _unitOfWork.ProductColorSizeStock.GetAvailableQuantity(model.ProductId, model.SelectedColor, model.SelectedSize);
                
                if (availableStock == 0)
                {
                    TempData["error"] = $"Sorry, {model.SelectedColor} color in size {model.SelectedSize} is currently out of stock.";
                    return RedirectToAction("Details", new { id = model.ProductId });
                }

                // Check if requested quantity exceeds available stock
                var existingCart = _unitOfWork.ShoppingCart.GetFirstorDefault(
                    u => u.ApplicationUserId == claim.Value &&
                         u.ProductId == model.ProductId &&
                         u.SelectedColor == model.SelectedColor &&
                         u.SelectedSize == model.SelectedSize
                );

                int totalRequestedQuantity = model.Count;
                if (existingCart != null)
                {
                    totalRequestedQuantity += existingCart.Count;
                }

                if (totalRequestedQuantity > availableStock)
                {
                    TempData["error"] = $"Only {availableStock} items available in stock for {model.SelectedColor} / {model.SelectedSize}.";
                    return RedirectToAction("Details", new { id = model.ProductId });
                }
            }

            var existingCartItem = _unitOfWork.ShoppingCart.GetFirstorDefault(
                u => u.ApplicationUserId == claim.Value &&
                     u.ProductId == model.ProductId &&
                     u.SelectedColor == model.SelectedColor &&
                     u.SelectedSize == model.SelectedSize
            );

            if (existingCartItem == null)
            {
                var newCartItem = new Shoppingcart
                {
                    ApplicationUserId = claim.Value,
                    ProductId = model.ProductId,
                    SelectedColor = model.SelectedColor,
                    SelectedSize = model.SelectedSize,
                    Count = model.Count
                };

                if (newCartItem.Count < 1) newCartItem.Count = 1;
                if (newCartItem.Count > 100) newCartItem.Count = 100;

                _unitOfWork.ShoppingCart.Add(newCartItem);
            }
            else
            {
                int newCount = existingCartItem.Count + model.Count;
                if (newCount > 100) newCount = 100;
                _unitOfWork.ShoppingCart.IncreaseCount(existingCartItem, model.Count);
            }


            // Update Session to be the sum of quantities
            HttpContext.Session.SetInt32(
                SD.SessionKey,
                _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Sum(x => x.Count)
            );

            _unitOfWork.Complete();
            TempData["success"] = "Product added to cart successfully!";
            return RedirectToAction("Index", "Cart", new { area = "Customer" });
        }
        
        public IActionResult Returns()
        {
            return View();
        }
    }
}