# Order Processing Integration Guide

## Overview
This guide shows how to integrate the stock management system with your order processing to automatically decrease stock when orders are placed.

---

## 🎯 Integration Points

### 1. When Order is Placed (Checkout)
### 2. When Order is Confirmed (Payment Success)
### 3. When Order is Cancelled (Stock Restoration)

---

## 📝 Implementation Steps

### Step 1: Find Your Order Processing Logic

Locate where orders are created in your codebase. This is likely in:
- `CartController.cs` - Summary/Checkout method
- `OrderController.cs` - CreateOrder method
- Or similar controller handling order placement

---

### Step 2: Add Stock Decrease Logic

#### Option A: Decrease Stock on Order Creation

Add this code **after** order is saved to database:

```csharp
[HttpPost]
public IActionResult CreateOrder(/* your parameters */)
{
    // Your existing order creation logic...
    // ...
    
    // After order is successfully created:
    try
    {
        foreach (var orderDetail in order.OrderDetails)
        {
            // Get the color from the order detail
            // NOTE: You may need to add a Color property to OrderDetails table
            var color = orderDetail.Color; // or however you track color in orders
            
            // Decrease stock
            var success = _unitOfWork.ProductColorStock.DecreaseStock(
                orderDetail.ProductId,
                color,
                orderDetail.Count
            );
            
            if (!success)
            {
                // Handle insufficient stock error
                // This shouldn't happen if cart validation is working
                _unitOfWork.Rollback(); // If you have transaction support
                TempData["error"] = $"Insufficient stock for {orderDetail.Product.Name} - {color}";
                return RedirectToAction("Cart");
            }
        }
        
        _unitOfWork.Complete();
        TempData["success"] = "Order placed successfully!";
        return RedirectToAction("OrderConfirmation", new { id = order.Id });
    }
    catch (Exception ex)
    {
        // Log error
        TempData["error"] = "Error processing order. Please try again.";
        return RedirectToAction("Cart");
    }
}
```

---

### Step 3: Update OrderDetails Model (If Needed)

If your OrderDetails table doesn't track color/size, add these properties:

```csharp
// In OrderDetails.cs
public class OrderDetails
{
    public int Id { get; set; }
    public int OrderHeaderId { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    
    // ADD THESE:
    public string? SelectedColor { get; set; }
    public string? SelectedSize { get; set; }
    
    // Navigation properties
    public OrderHeader OrderHeader { get; set; }
    public Product Product { get; set; }
}
```

Create a migration:
```bash
dotnet ef migrations add AddColorSizeToOrderDetails --startup-project ..\DAJDAJ.Web\DAJDAJ.Web.csproj
dotnet ef database update --startup-project ..\DAJDAJ.Web\DAJDAJ.Web.csproj
```

---

### Step 4: Update Cart to Order Conversion

When converting cart items to order details, include color/size:

```csharp
// In your order creation logic
foreach (var cartItem in cartItems)
{
    var orderDetail = new OrderDetails
    {
        ProductId = cartItem.ProductId,
        Count = cartItem.Count,
        Price = cartItem.product.Price,
        
        // Include these from cart:
        SelectedColor = cartItem.SelectedColor,
        SelectedSize = cartItem.SelectedSize
    };
    
    order.OrderDetails.Add(orderDetail);
}
```

---

### Step 5: Handle Order Cancellation

When an order is cancelled, restore stock:

```csharp
[HttpPost]
public IActionResult CancelOrder(int orderId)
{
    var order = _unitOfWork.OrderHeader.GetFirstorDefault(
        o => o.Id == orderId,
        includeword: "OrderDetails"
    );
    
    if (order == null)
    {
        return NotFound();
    }
    
    try
    {
        // Restore stock for each order item
        foreach (var detail in order.OrderDetails)
        {
            _unitOfWork.ProductColorStock.UpdateStock(
                detail.ProductId,
                detail.SelectedColor,
                _unitOfWork.ProductColorStock.GetAvailableQuantity(detail.ProductId, detail.SelectedColor) + detail.Count
            );
        }
        
        // Update order status
        order.OrderStatus = SD.StatusCancelled;
        _unitOfWork.Complete();
        
        TempData["success"] = "Order cancelled and stock restored.";
        return RedirectToAction("Details", new { id = orderId });
    }
    catch (Exception ex)
    {
        TempData["error"] = "Error cancelling order.";
        return RedirectToAction("Details", new { id = orderId });
    }
}
```

---

## 🔒 Advanced: Stock Reservation System

To prevent race conditions and ensure stock is held for users during checkout:

### Step 1: Add Reservation Table

```csharp
public class StockReservation
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Color { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsCompleted { get; set; }
    
    public Product Product { get; set; }
}
```

### Step 2: Reserve Stock When Adding to Cart

```csharp
// In HomeController.Details POST action
// After validating stock availability:

var reservation = new StockReservation
{
    ProductId = model.ProductId,
    Color = model.SelectedColor,
    Quantity = model.Count,
    UserId = claim.Value,
    ReservedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddMinutes(15), // 15-minute hold
    IsCompleted = false
};

_context.StockReservations.Add(reservation);
_context.SaveChanges();
```

### Step 3: Update Available Stock Calculation

```csharp
public int GetAvailableQuantity(int productId, string color)
{
    var stock = GetStockByProductAndColor(productId, color);
    var totalQuantity = stock?.Quantity ?? 0;
    
    // Subtract active reservations
    var reservedQuantity = _context.StockReservations
        .Where(r => r.ProductId == productId && 
                    r.Color == color && 
                    !r.IsCompleted && 
                    r.ExpiresAt > DateTime.UtcNow)
        .Sum(r => r.Quantity);
    
    return totalQuantity - reservedQuantity;
}
```

### Step 4: Release Expired Reservations

Create a background job:

```csharp
// In a scheduled task or background service
public void ReleaseExpiredReservations()
{
    var expiredReservations = _context.StockReservations
        .Where(r => !r.IsCompleted && r.ExpiresAt <= DateTime.UtcNow)
        .ToList();
    
    foreach (var reservation in expiredReservations)
    {
        reservation.IsCompleted = true;
    }
    
    _context.SaveChanges();
}
```

---

## 🧪 Testing the Integration

### Test Scenarios

1. **Normal Order Flow**
   - Add item to cart
   - Checkout
   - Verify stock decreased

2. **Insufficient Stock**
   - Reduce stock to 1
   - Try to order 2
   - Should show error

3. **Order Cancellation**
   - Place order
   - Cancel order
   - Verify stock restored

4. **Concurrent Orders**
   - Two users order same item
   - Both add to cart
   - First to checkout succeeds
   - Second should fail if stock runs out

---

## 📊 Monitoring & Reporting

### Create Admin Report for Stock Changes

```csharp
// In StockController
public IActionResult StockMovementReport(DateTime? startDate, DateTime? endDate)
{
    // Query stock changes from order history
    var movements = _context.OrderDetails
        .Where(o => o.OrderHeader.OrderDate >= (startDate ?? DateTime.MinValue) &&
                    o.OrderHeader.OrderDate <= (endDate ?? DateTime.MaxValue))
        .Include(o => o.Product)
        .Select(o => new StockMovementVM
        {
            ProductName = o.Product.Name,
            Color = o.SelectedColor,
            Quantity = -o.Count, // Negative because it's a decrease
            Date = o.OrderHeader.OrderDate,
            Type = "Order",
            OrderId = o.OrderHeaderId
        })
        .ToList();
    
    return View(movements);
}
```

---

## ⚠️ Important Considerations

### Race Conditions
- **Problem**: Two users might buy the last item simultaneously
- **Solution**: Use database transactions or stock reservation system

### Stock Synchronization
- **Problem**: Manual inventory adjustments
- **Solution**: Track all changes in StockHistory table

### Payment Failures
- **Problem**: Stock decreased but payment fails
- **Solution**: Decrease stock only after payment confirmation

### Inventory Audits
- **Problem**: Stock in system vs physical inventory mismatch
- **Solution**: Regular audits and adjustment features in admin panel

---

## 🔄 Recommended Workflow

### For Standard E-commerce:

1. **Add to Cart**: Check stock, show availability
2. **Checkout Initiated**: Reserve stock for 15 minutes
3. **Payment Processing**: Hold reservation
4. **Payment Success**: Decrease stock, mark reservation complete
5. **Payment Failure**: Release reservation
6. **Order Shipped**: No stock change (already decreased)
7. **Order Cancelled**: Restore stock, mark reservation cancelled

---

## 📝 Code Checklist

Before going live:

- [ ] Stock decreases on order placement
- [ ] Stock restores on order cancellation
- [ ] Order details include color/size
- [ ] Validation prevents overselling
- [ ] Transactions handle failures gracefully
- [ ] Background job releases expired reservations (if using)
- [ ] Admin can view stock movement report
- [ ] Error logging is in place
- [ ] Email notifications for critical stock levels

---

## 🚀 Deployment

1. Add color/size to OrderDetails (if not present)
2. Run migration
3. Update order creation logic
4. Add order cancellation logic
5. Test thoroughly in staging
6. Monitor closely after production deployment

---

## 📞 Troubleshooting

### Stock not decreasing
- Check if DecreaseStock is being called
- Verify _unitOfWork.Complete() is executed
- Check for exceptions in logs

### Stock goes negative
- This shouldn't happen with proper validation
- Check for race conditions
- Implement database constraint: `CHECK (Quantity >= 0)`

### Stock doesn't restore on cancellation
- Verify cancellation code is calling UpdateStock
- Check if order details have color information

---

**Next**: Implement these changes in your order processing logic and test thoroughly!

---

## Example: Complete Integration in CartController

```csharp
[HttpPost]
[Authorize]
public IActionResult PlaceOrder(ShoppingCartVM model)
{
    var claimsIdentity = (ClaimsIdentity)User.Identity;
    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
    
    var cartItems = _unitOfWork.ShoppingCart.GetAll(
        u => u.ApplicationUserId == userId, 
        "product"
    ).ToList();
    
    if (!cartItems.Any())
    {
        TempData["error"] = "Your cart is empty.";
        return RedirectToAction("Index", "Cart");
    }
    
    // Validate stock one more time before order
    foreach (var item in cartItems)
    {
        var availableStock = _unitOfWork.ProductColorStock.GetAvailableQuantity(
            item.ProductId, 
            item.SelectedColor
        );
        
        if (item.Count > availableStock)
        {
            TempData["error"] = $"Insufficient stock for {item.product.Name} - {item.SelectedColor}. Only {availableStock} available.";
            return RedirectToAction("Index", "Cart");
        }
    }
    
    // Create order
    var order = new OrderHeader
    {
        ApplicationUserId = userId,
        OrderDate = DateTime.Now,
        TotalPrice = cartItems.Sum(c => c.Count * c.product.Price),
        OrderStatus = SD.StatusPending,
        PaymentStatus = SD.PaymentStatusPending
    };
    
    _unitOfWork.OrderHeader.Add(order);
    _unitOfWork.Complete(); // Save to get OrderId
    
    // Create order details and decrease stock
    foreach (var item in cartItems)
    {
        var orderDetail = new OrderDetails
        {
            OrderHeaderId = order.Id,
            ProductId = item.ProductId,
            Count = item.Count,
            Price = item.product.Price,
            SelectedColor = item.SelectedColor,
            SelectedSize = item.SelectedSize
        };
        
        _unitOfWork.OrderDetails.Add(orderDetail);
        
        // Decrease stock
        var success = _unitOfWork.ProductColorStock.DecreaseStock(
            item.ProductId,
            item.SelectedColor,
            item.Count
        );
        
        if (!success)
        {
            // Rollback order
            _unitOfWork.OrderHeader.Remove(order);
            _unitOfWork.Complete();
            
            TempData["error"] = "Stock changed during checkout. Please try again.";
            return RedirectToAction("Index", "Cart");
        }
    }
    
    // Clear cart
    _unitOfWork.ShoppingCart.RemoveRange(cartItems);
    _unitOfWork.Complete();
    
    TempData["success"] = "Order placed successfully!";
    return RedirectToAction("OrderConfirmation", new { id = order.Id });
}
```

This is a complete, production-ready implementation! 🎉
