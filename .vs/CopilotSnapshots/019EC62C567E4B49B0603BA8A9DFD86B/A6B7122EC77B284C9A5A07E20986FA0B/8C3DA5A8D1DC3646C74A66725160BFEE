using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DAJDAJ.Web.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OrderApiController : ControllerBase
    {
        private readonly IUntiOfWork _untiOfWork;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public OrderApiController(IUntiOfWork unitOfWork, IConfiguration configuration)
        {
            _untiOfWork = unitOfWork;
            _configuration = configuration;
            _secretKey = _configuration["Authentication:ApiSettings:SecretKey"];
        }

        [HttpPost("update-multiple-status")]
        public IActionResult UpdateMultipleOrdersStatus([FromHeader] string apiKey, [FromBody] UpdateMultipleOrdersRequest request)
        {
            if (apiKey != _secretKey)
                return Unauthorized("Invalid API Key");

            if (request.OrderIds == null || !request.OrderIds.Any())
                return BadRequest("No order IDs provided.");

            var validStatuses = new[]
            {
                SD.Pending, SD.Approve, SD.Proccessing, SD.Cancelled,
                SD.Shipped, SD.Refund, SD.Rejected, SD.Confirmed,
                SD.Return, SD.Earned, SD.Booked
            };

            if (!validStatuses.Contains(request.Status))
                return BadRequest("Invalid status value");

            int updated = 0;

            foreach (var orderId in request.OrderIds)
            {
                var order = _untiOfWork.OrderHeader.GetFirstorDefault(o => o.Id == orderId);
                if (order != null)
                {
                    order.OrderStatus = request.Status;
                    _untiOfWork.OrderHeader.Update(order);
                    updated++;
                }
            }

            _untiOfWork.Complete();

            return Ok(new
            {
                message = $"✅ {updated} orders updated successfully to '{request.Status}'."
            });
        }

        [HttpGet("get-booked")]
        public IActionResult GetBookedOrders([FromQuery] string apiKey)
        {
            if (apiKey != _secretKey)
                return Unauthorized("Invalid API Key");

            var bookedOrders = _untiOfWork.OrderHeader.GetAll(o => o.OrderStatus == SD.Booked, "OrderDetails,OrderDetails.Product")
                   .Select(o => new OrderForShippingDto
                   {
                       Id = o.Id,
                       Name = o.Name,
                       Address = o.Address,
                       City = o.City,
                       Phone = o.Phone,
                       OrderStatus = o.OrderStatus,
                       TotalPrice = o.TotalPrice,
                       Items = o.OrderDetails.Select(d => new OrderItemDto
                       {
                           ProductName = d.Product.Name, 
                           Color = d.SelectedColor,
                           Size = d.SelectedSize,
                           Quantity = d.Count,
                           Price = d.Price
                       }).ToList()
                   }).ToList();
            if (!bookedOrders.Any())
                return NotFound("No booked orders found.");

            return Ok(bookedOrders);
        }

        [HttpGet("get-cancelled")]
        public IActionResult GetCancelledOrders([FromQuery] string apiKey)
        {
            if (apiKey != _secretKey)
                return Unauthorized("Invalid API Key");

            var cancelledOrders = _untiOfWork.OrderHeader.GetAll(o => o.OrderStatus == SD.Cancelled, "OrderDetails,OrderDetails.Product")
                             .Select(o => new OrderForShippingDto
                             {
                                 Id = o.Id,
                                 Name = o.Name,
                                 Address = o.Address,
                                 City = o.City,
                                 Phone = o.Phone,
                                 OrderStatus = o.OrderStatus,
                                 TotalPrice = o.TotalPrice,
                                 Items = o.OrderDetails.Select(d => new OrderItemDto
                                 {
                                     ProductName = d.Product.Name,
                                     Color = d.SelectedColor,
                                     Size = d.SelectedSize,
                                     Quantity = d.Count,
                                     Price = d.Price
                                 }).ToList()
                             }).ToList();
            if (!cancelledOrders.Any())
                return NotFound("No cancelled orders found.");

            return Ok(cancelledOrders);
        }

        [HttpGet("get-earned")]
        public IActionResult GetEarnedOrders([FromQuery] string apiKey)
        {
            if (apiKey != _secretKey)
                return Unauthorized("Invalid API Key");
            var earnedOrders = _untiOfWork.OrderHeader.GetAll(o => o.OrderStatus == SD.Earned, "OrderDetails,OrderDetails.Product")
       .Select(o => new OrderForShippingDto
       {
           Id = o.Id,
           Name = o.Name,
           Address = o.Address,
           City = o.City,
           Phone = o.Phone,
           OrderStatus = o.OrderStatus,
           TotalPrice = o.TotalPrice,
           Items = o.OrderDetails.Select(d => new OrderItemDto
           {
               ProductName = d.Product.Name,
               Color = d.SelectedColor,
               Size = d.SelectedSize,
               Quantity = d.Count,
               Price = d.Price
           }).ToList()
       }).ToList();

            if (!earnedOrders.Any())
                return NotFound("No earned orders found.");

            return Ok(earnedOrders);
        }
    }

    public class UpdateMultipleOrdersRequest
    {
        public string ApiKey { get; set; }
        public List<int> OrderIds { get; set; }
        public string Status { get; set; }
    }
}
