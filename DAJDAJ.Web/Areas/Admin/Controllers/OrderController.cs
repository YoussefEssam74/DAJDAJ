using DAJDAJ.DataAccess;
using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Entities.ViewModels;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OfficeOpenXml;
using System.IO;
using System.Linq;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ("Admin"))]

    public class OrderController : Controller
    {
        private readonly IUntiOfWork _untiOfWork;
        [BindProperty]
        public OrderVM? OrderVM { get; set; }
        public OrderController(IUntiOfWork untiOfWork)
        {
            _untiOfWork = untiOfWork;
        }

        // Helper method to strip HTML tags and entities from text
        private string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            
            // Remove HTML tags
            var withoutTags = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
            
            // Decode HTML entities like &nbsp;
            var decoded = System.Net.WebUtility.HtmlDecode(withoutTags);
            
            // Trim and return
            return decoded.Trim();
        }

        // Helper method to translate colors and sizes to Arabic
        private string TranslateToArabic(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Colors
                {"Red", "أحمر"},
                {"Blue", "أزرق"},
                {"Green", "أخضر"},
                {"Black", "أسود"},
                {"White", "أبيض"},
                {"Yellow", "أصفر"},
                {"Orange", "برتقالي"},
                {"Purple", "بنفسجي"},
                {"Pink", "زهري"},
                {"Brown", "بني"},
                {"Gray", "رمادي"},
                {"Grey", "رمادي"},
                {"Beige", "بيج"},
                {"Navy", "كحلي"},
                {"Burgundy", "نبيتى"},
                {"Maroon", "عنابي"},
                {"Gold", "ذهبي"},
                {"Silver", "فضي"},
                {"Khaki", "كاكي"},
                {"Olive", "زيتوني"},
                {"Turquoise", "تركواز"},
                {"Cream", "كريمي"},
                {"Ivory", "عاجي"},
                
                // Cities
                {"Cairo", "القاهرة"},
                {"Giza", "الجيزة"},
                {"Alexandria", "الإسكندرية"},
                {"Fayoum", "الفيوم"},
                {"Beni Suef", "بني سويف"},
                {"Minya", "المنيا"},
                {"Assiut", "أسيوط"},
                {"Sohag", "سوهاج"},
                {"Qena", "قنا"},
                {"Luxor", "الأقصر"},
                {"Aswan", "أسوان"},
                {"Port Said", "بورسعيد"},
                {"Suez", "السويس"},
                {"Ismailia", "الإسماعيلية"},
                {"Damietta", "دمياط"},
                {"Dakahlia", "الدقهلية"},
                {"Mansoura", "المنصورة"},
                {"Sharqia", "الشرقية"},
                {"Zagazig", "الزقازيق"},
                {"Monufia", "المنوفية"},
                {"Qalyubia", "القليوبية"},
                {"Kafr El Sheikh", "كفر الشيخ"},
                {"Gharbia", "الغربية"},
                {"Tanta", "طنطا"},
                {"Beheira", "البحيرة"},
                {"Damanhur", "دمنهور"},
                {"Matrouh", "مطروح"},
                {"North Sinai", "شمال سيناء"},
                {"South Sinai", "جنوب سيناء"},
                {"Sharm El Sheikh", "شرم الشيخ"},
                {"Hurghada", "الغردقة"},
                {"Red Sea", "البحر الأحمر"},
                {"New Valley", "الوادي الجديد"},
                {"Gouna", "الجونة"},
                {"Sahel", "الساحل"},
            };

            // If exact match exists, return it
            if (translations.ContainsKey(text))
                return translations[text];

            // Otherwise, translate keywords in the text
            var keywordTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Leather Jacket", "جاكيت جلد"},
                {"Denim Jacket", "جاكيت جينز"},
                {"Puffer Jacket", "جاكيت مبطن"},
                {"Jacket", "جاكيت"},
                {"Blazer", "بليزر"},
                {"Coat", "معطف"},
                {"Leather", "جلد"},
                {"Suede", "سويدى"},
                {"Pullover", "بلوفر"},
                {"Sweater", "سويتر"},
                {"Zipper", "سوسته"},
                {"Double", "مزدوج"},
                {"Long Sleeve", "أكمام طويلة"},
                {"Short Sleeve", "أكمام قصيرة"},
                {"Stripped", "مخطط"},
                {"Striped", "مخطط"},
                {"T-Shirt", "تي شيرت"},
                {"Shirt", "قميص"},
                {"Pants", "بنطلون"},
                {"Jeans", "جينز"},
                {"Dress", "فستان"},
                {"Skirt", "تنورة"},
                {"Hoodie", "هودي"},
                {"Cardigan", "كارديجان"},
                {"Vest", "صديري"},
                {"Shorts", "شورت"},
            };

            string result = text;
            foreach (var keyword in keywordTranslations)
            {
                result = System.Text.RegularExpressions.Regex.Replace(
                    result, 
                    keyword.Key, 
                    keyword.Value, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return result;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BulkStatusUpdate()
        {
            ViewBag.PageTitle = "Bulk Status Update";
            ViewBag.CardTitle = "Update Multiple Orders Status";
            
            // Pass available statuses to the view
            ViewBag.Statuses = new List<string>
            {
                SD.Pending, SD.Approve, SD.Proccessing, SD.Cancelled,
                SD.Shipped, SD.Refund, SD.Rejected, SD.Confirmed,
                SD.Return, SD.Earned, SD.Booked
            };
            
            return View();
        }

        public IActionResult GetData(string? status, DateTime? startDate, DateTime? endDate, string? customerName, string? phone)
        {
            var ordersQuery = _untiOfWork.OrderHeader.GetAll();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Name.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                ordersQuery = ordersQuery.Where(o => o.Phone.Contains(phone));
            }

            var orders = ordersQuery
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
            if (OrderVM == null || OrderVM.OrderHeader == null)
            {
                TempData["Error"] = "Invalid order data.";
                return RedirectToAction("Index");
            }

            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            
            if (orderFromDb == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.Phone = OrderVM.OrderHeader.Phone;
            orderFromDb.Address = StripHtml(OrderVM.OrderHeader.Address);
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
            _untiOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Proccessing, string.Empty);
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
        public IActionResult BookOrder(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderFromDb == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
            }

            // Idempotency check: Prevent booking if not in Processing status
            if (orderFromDb.OrderStatus != SD.Proccessing)
            {
                TempData["Error"] = $"Cannot book order. Current status is '{orderFromDb.OrderStatus}'.";
                return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
            }

            orderFromDb.OrderStatus = SD.Booked;

            _untiOfWork.OrderHeader.Update(orderFromDb);
            _untiOfWork.Complete();

            TempData["Update"] = "Order has been booked successfully.";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EarnOrder(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderFromDb == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
            }

            // Idempotency check: Prevent earning if already earned
            if (orderFromDb.OrderStatus == SD.Earned)
            {
                TempData["Error"] = "Order has already been marked as earned.";
                return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
            }

            orderFromDb.OrderStatus = SD.Earned;

            _untiOfWork.OrderHeader.Update(orderFromDb);
            _untiOfWork.Complete();

            TempData["Update"] = "Order has been marked as earned successfully.";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnOrder(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderFromDb == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
            }

            // Idempotency check: Prevent returning if already returned
            if (orderFromDb.OrderStatus == SD.Return)
            {
                TempData["Error"] = "Order has already been returned.";
                return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
            }

            orderFromDb.OrderStatus = SD.Return;

            _untiOfWork.OrderHeader.Update(orderFromDb);
            _untiOfWork.Complete();

            TempData["Update"] = "Order has been returned successfully.";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(OrderVM OrderVM)
        {
            var orderFromDb = _untiOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            
            if (orderFromDb == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
            }

            // Idempotency check: Prevent cancelling if already cancelled or refunded
            if (orderFromDb.OrderStatus == SD.Cancelled || orderFromDb.OrderStatus == SD.Refund)
            {
                TempData["Error"] = $"Order has already been {orderFromDb.OrderStatus.ToLower()}.";
                return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
            }

            if (orderFromDb.PaymentMethod == "CashOnDelivery")
            {
                var orderDetails = _untiOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id).ToList();
                
                foreach (var item in orderDetails)
                {
                    if (!string.IsNullOrEmpty(item.SelectedColor))
                    {
                        var currentStock = _untiOfWork.ProductColorStock.GetStockByProductAndColor(item.ProductId, item.SelectedColor);
                        if (currentStock != null)
                        {
                            _untiOfWork.ProductColorStock.UpdateStock(
                                item.ProductId,
                                item.SelectedColor,
                                currentStock.Quantity + item.Count
                            );
                        }
                    }
                }
                
                _untiOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, string.Empty);
            }

            _untiOfWork.Complete();


            TempData["Update"] = "Order has been Cancelled successfully.";

            return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var orders = _untiOfWork.OrderHeader.GetAll().ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                
                // Arabic column headers
                worksheet.Cells[1, 1].Value = "كود التاجر";
                worksheet.Cells[1, 2].Value = "اسم الراسل علي البوليصة";
                worksheet.Cells[1, 3].Value = "اسم المستلم";
                worksheet.Cells[1, 4].Value = "موبايل المستلم";
                worksheet.Cells[1, 5].Value = "ملاحظات";
                worksheet.Cells[1, 6].Value = "العنوان";
                worksheet.Cells[1, 7].Value = "محتوى الشحنة";
                worksheet.Cells[1, 8].Value = "الكمية";
                worksheet.Cells[1, 9].Value = "قيمة الشُّـحنه";

                int row = 2;
                foreach (var order in orders)
                {
                    // Get order details with products
                    var orderDetails = _untiOfWork.OrderDetails.GetAll(d => d.OrderHeaderId == order.Id, "Product").ToList();
                    
                    // Build product details string with Arabic translations
                    var productInfo = string.Join("\n", orderDetails.Select(d => 
                        $"{TranslateToArabic(d.Product.Name)} - {TranslateToArabic(d.SelectedColor)} - {TranslateToArabic(d.SelectedSize)} (x{d.Count})"
                    ));
                    
                    // Calculate total quantity
                    var totalQuantity = orderDetails.Sum(d => d.Count);
                    
                    // Calculate shipping cost based on city
                    decimal shippingCost = 80; // Default
                    var city = order.City.ToLower().Trim();
                    
                    if (city == "cairo" || city == "giza")
                    {
                        shippingCost = 70;
                    }
                    else if (city == "fayoum" || city == "beni suef" || city == "minya" || 
                             city == "assiut" || city == "sohag" || city == "qena" || 
                             city == "luxor" || city == "aswan")
                    {
                        shippingCost = 100;
                    }
                    else if (city == "gouna" || city == "sahel" || city == "red sea" || 
                             city == "sharm el sheikh" || city == "new valley")
                    {
                        shippingCost = 130;
                    }

                    worksheet.Cells[row, 1].Value = order.Id;
                    worksheet.Cells[row, 2].Value = "dajdaj.eg";
                    worksheet.Cells[row, 3].Value = order.Name;
                    worksheet.Cells[row, 4].Value = order.Phone;
                    worksheet.Cells[row, 5].Value = $"عند رفض الاستلام دفع {shippingCost} شحن";
                    worksheet.Cells[row, 6].Value = $"{order.Address}, {TranslateToArabic(order.City)}";
                    worksheet.Cells[row, 7].Value = productInfo;
                    worksheet.Cells[row, 8].Value = totalQuantity;
                    worksheet.Cells[row, 9].Value = order.TotalPrice;
                    
                    // Enable text wrapping for product content column
                    worksheet.Cells[row, 7].Style.WrapText = true;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Column(7).Width = 50; // Set product content column width

                var stream = new MemoryStream(package.GetAsByteArray());
                string excelName = $"DAJDAJ-Orders-{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public IActionResult ExportSelectedToExcel([FromForm] List<int> orderIds)
        {
            var orders = _untiOfWork.OrderHeader.GetAll(o => orderIds.Contains(o.Id)).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Address, City";
                worksheet.Cells[1, 5].Value = "Order Status";
                worksheet.Cells[1, 6].Value = "Total Price";

                for (int i = 0; i < orders.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = orders[i].Id;
                    worksheet.Cells[i + 2, 2].Value = orders[i].Name;
                    worksheet.Cells[i + 2, 3].Value = orders[i].Phone;
                    worksheet.Cells[i + 2, 4].Value = $"{orders[i].Address}, {orders[i].City}";
                    worksheet.Cells[i + 2, 5].Value = orders[i].OrderStatus;
                    worksheet.Cells[i + 2, 6].Value = orders[i].TotalPrice;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                string excelName = $"DAJDAJ-Orders-Page-{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpGet]
        public IActionResult ExportFilteredToExcel(string? status, DateTime? startDate, DateTime? endDate, string? customerName, string? phone, int? fromId, int? toId)
        {
            var ordersQuery = _untiOfWork.OrderHeader.GetAll();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Name.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                ordersQuery = ordersQuery.Where(o => o.Phone.Contains(phone));
            }

            // Filter by ID range
            if (fromId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Id >= fromId.Value);
            }

            if (toId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Id <= toId.Value);
            }

            var orders = ordersQuery.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                
                // Arabic column headers
                worksheet.Cells[1, 1].Value = "كود التاجر";
                worksheet.Cells[1, 2].Value = "اسم الراسل علي البوليصة";
                worksheet.Cells[1, 3].Value = "اسم المستلم";
                worksheet.Cells[1, 4].Value = "موبايل المستلم";
                worksheet.Cells[1, 5].Value = "ملاحظات";
                worksheet.Cells[1, 6].Value = "العنوان";
                worksheet.Cells[1, 7].Value = "محتوى الشحنة";
                worksheet.Cells[1, 8].Value = "الكمية";
                worksheet.Cells[1, 9].Value = "قيمة الشُّـحنه";

                int row = 2;
                foreach (var order in orders)
                {
                    // Get order details with products
                    var orderDetails = _untiOfWork.OrderDetails.GetAll(d => d.OrderHeaderId == order.Id, "Product").ToList();
                    
                    // Build product details string with Arabic translations
                    var productInfo = string.Join("\n", orderDetails.Select(d => 
                        $"{TranslateToArabic(d.Product.Name)} - {TranslateToArabic(d.SelectedColor)} - {TranslateToArabic(d.SelectedSize)} (x{d.Count})"
                    ));
                    
                    // Calculate total quantity
                    var totalQuantity = orderDetails.Sum(d => d.Count);
                    
                    // Calculate shipping cost based on city
                    decimal shippingCost = 80; // Default
                    var city = order.City.ToLower().Trim();
                    
                    if (city == "cairo" || city == "giza")
                    {
                        shippingCost = 70;
                    }
                    else if (city == "fayoum" || city == "beni suef" || city == "minya" || 
                             city == "assiut" || city == "sohag" || city == "qena" || 
                             city == "luxor" || city == "aswan")
                    {
                        shippingCost = 100;
                    }
                    else if (city == "gouna" || city == "sahel" || city == "red sea" || 
                             city == "sharm el sheikh" || city == "new valley")
                    {
                        shippingCost = 130;
                    }

                    worksheet.Cells[row, 1].Value = order.Id;
                    worksheet.Cells[row, 2].Value = "dajdaj.eg";
                    worksheet.Cells[row, 3].Value = order.Name;
                    worksheet.Cells[row, 4].Value = order.Phone;
                    worksheet.Cells[row, 5].Value = $"عند رفض الاستلام دفع {shippingCost} شحن";
                    worksheet.Cells[row, 6].Value = $"{order.Address}, {TranslateToArabic(order.City)}";
                    worksheet.Cells[row, 7].Value = productInfo;
                    worksheet.Cells[row, 8].Value = totalQuantity;
                    worksheet.Cells[row, 9].Value = order.TotalPrice;
                    
                    // Enable text wrapping for product content column
                    worksheet.Cells[row, 7].Style.WrapText = true;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Column(7).Width = 50; // Set product content column width

                var stream = new MemoryStream(package.GetAsByteArray());
                string excelName = $"DAJDAJ-Filtered-Orders-{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsCompleted(string? status, DateTime? startDate, DateTime? endDate, string? customerName, string? phone)
        {
            var ordersQuery = _untiOfWork.OrderHeader.GetAll();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Name.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                ordersQuery = ordersQuery.Where(o => o.Phone.Contains(phone));
            }

            var orders = ordersQuery.ToList();
            int updatedCount = 0;

            foreach (var order in orders)
            {
                if (order.OrderStatus != SD.Earned)
                {
                    order.OrderStatus = SD.Earned;
                    _untiOfWork.OrderHeader.Update(order);
                    updatedCount++;
                }
            }

            _untiOfWork.Complete();

            TempData["Update"] = $"{updatedCount} order(s) marked as completed successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult PrintFiltered(string? status, DateTime? startDate, DateTime? endDate, string? customerName, string? phone, bool? onlyUnprinted = null)
        {
            var ordersQuery = _untiOfWork.OrderHeader.GetAll();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }

            // Filter by IsPrinted if specified
            if (onlyUnprinted.HasValue && onlyUnprinted.Value)
            {
                ordersQuery = ordersQuery.Where(o => !o.IsPrinted);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Name.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                ordersQuery = ordersQuery.Where(o => o.Phone.Contains(phone));
            }

            var orderIds = ordersQuery.Select(o => o.Id).ToList();

            var ordersVM = new List<OrderVM>();
            foreach (var orderId in orderIds)
            {
                var orderVM = new OrderVM
                {
                    OrderHeader = _untiOfWork.OrderHeader.GetFirstorDefault(o => o.Id == orderId),
                    OrderDetails = _untiOfWork.OrderDetails.GetAll(d => d.OrderHeaderId == orderId, "Product")
                };
                ordersVM.Add(orderVM);
            }

            // Pass the order IDs to the view so they can be marked as printed after actual printing
            ViewBag.OrderIds = orderIds;
            ViewBag.OnlyUnprinted = onlyUnprinted;

            return View(ordersVM);
        }

        [HttpPost]
        public IActionResult MarkOrdersAsPrinted([FromBody] List<int> orderIds)
        {
            try
            {
                foreach (var orderId in orderIds)
                {
                    var order = _untiOfWork.OrderHeader.GetFirstorDefault(o => o.Id == orderId);
                    if (order != null)
                    {
                        order.IsPrinted = true;
                    }
                }
                _untiOfWork.Complete();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAllStatus(string newStatus, string? status, DateTime? startDate, DateTime? endDate, string? customerName, string? phone)
        {
            if (string.IsNullOrEmpty(newStatus))
            {
                return Json(new { success = false, message = "Please select a status to change to." });
            }

            var ordersQuery = _untiOfWork.OrderHeader.GetAll();

            // Apply the same filters
            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Name.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                ordersQuery = ordersQuery.Where(o => o.Phone.Contains(phone));
            }

            var orders = ordersQuery.ToList();
            int updatedCount = 0;

            foreach (var order in orders)
            {
                order.OrderStatus = newStatus;
                
                // Reset IsPrinted when changing to Booked status
                if (newStatus == SD.Booked)
                {
                    order.IsPrinted = false;
                }
                
                _untiOfWork.OrderHeader.Update(order);
                updatedCount++;
            }

            _untiOfWork.Complete();

            return Json(new { success = true, message = $"{updatedCount} order(s) status changed to '{newStatus}' successfully." });
        }

    }
}
