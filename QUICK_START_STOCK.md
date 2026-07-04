# Quick Start: Adding Stock Data

## Prerequisites
- Stock Management system is installed
- Database migration has been applied
- You have admin access to the system

---

## Method 1: Via Admin UI (Recommended)

### Step 1: Access Stock Management
1. Login as Admin
2. Navigate to `/Admin/Stock/Index`

### Step 2: Add Stock for Each Product
For each product:
1. Click **"Add"** button
2. Select a color from the dropdown
3. Enter the quantity
4. Click **"Add Stock"**
5. Repeat for each color

---

## Method 2: Via Database (Bulk Insert)

If you have many products, you can insert stock data directly via SQL:

```sql
-- Example: Add stock for Product ID 1
INSERT INTO ProductColorStocks (ProductId, Color, Quantity, LastUpdated)
VALUES 
    (1, 'Black', 50, GETUTCDATE()),
    (1, 'White', 30, GETUTCDATE()),
    (1, 'Red', 20, GETUTCDATE());

-- Example: Add stock for multiple products
INSERT INTO ProductColorStocks (ProductId, Color, Quantity, LastUpdated)
VALUES 
    (2, 'Blue', 15, GETUTCDATE()),
    (2, 'Green', 25, GETUTCDATE()),
    (3, 'Pink', 10, GETUTCDATE()),
    (3, 'Yellow', 5, GETUTCDATE());
```

### Important Notes:
- `ProductId` must match an existing product
- `Color` should match the colors in ProductImages table
- `Quantity` = 0 marks the color as sold out
- `LastUpdated` should be set to current UTC time

---

## Method 3: Via Code (Programmatic)

You can create a seeder service to populate initial stock:

```csharp
// In DbIntializar or a separate seeder class
public void SeedProductStock()
{
    // Example: Add stock for all products
    var products = _context.products.Include(p => p.ProductImages).ToList();
    
    foreach (var product in products)
    {
        var colors = product.ProductImages.Select(img => img.Color).Distinct();
        
        foreach (var color in colors)
        {
            var existingStock = _context.ProductColorStocks
                .FirstOrDefault(s => s.ProductId == product.Id && s.Color == color);
            
            if (existingStock == null)
            {
                _context.ProductColorStocks.Add(new ProductColorStock
                {
                    ProductId = product.Id,
                    Color = color,
                    Quantity = 50, // Default quantity
                    LastUpdated = DateTime.UtcNow
                });
            }
        }
    }
    
    _context.SaveChanges();
}
```

---

## Sample Stock Data Template

If you're using Excel or CSV, use this format:

| ProductId | Color  | Quantity |
|-----------|--------|----------|
| 1         | Black  | 50       |
| 1         | White  | 30       |
| 1         | Red    | 20       |
| 2         | Blue   | 15       |
| 2         | Green  | 25       |
| 3         | Pink   | 10       |
| 3         | Yellow | 5        |

Convert to SQL INSERT statements or import via tool.

---

## Verification Steps

After adding stock:

1. ✅ Go to `/Admin/Stock/Index`
2. ✅ Verify all products show stock data
3. ✅ Check product detail pages show correct stock
4. ✅ Try adding a product to cart
5. ✅ Verify low stock alerts appear for quantities ≤ 5

---

## Common Issues

### Colors don't appear in dropdown
**Cause**: No ProductImages exist for the product
**Solution**: Add ProductImage entries first with Color property set

### Stock doesn't show on product page
**Cause**: Stock entry doesn't match color exactly (case-sensitive)
**Solution**: Ensure Color in ProductColorStocks exactly matches Color in ProductImages

### All products show as sold out
**Cause**: No stock entries exist
**Solution**: Add stock entries with Quantity > 0

---

## Recommended Initial Quantities

- **New Products**: 30-50 units per color
- **Popular Items**: 100+ units per color
- **Limited Edition**: 5-10 units per color (triggers low stock alert)
- **Sold Out**: 0 units

---

## Maintenance Tips

1. **Weekly Review**: Check low stock alerts
2. **Monthly Audit**: Verify stock counts match actual inventory
3. **Before Promotions**: Increase stock for featured products
4. **After Orders**: Stock is automatically decreased (implement in order processing)

---

## Next: Implement Order Processing Integration

When an order is placed, decrease stock:

```csharp
// In Order processing logic
foreach (var orderDetail in order.OrderDetails)
{
    var product = orderDetail.Product;
    var color = orderDetail.Color; // Assuming you track color in order
    
    var success = _unitOfWork.ProductColorStock.DecreaseStock(
        product.Id, 
        color, 
        orderDetail.Quantity
    );
    
    if (!success)
    {
        // Handle insufficient stock error
        throw new InvalidOperationException("Insufficient stock");
    }
}

_unitOfWork.Complete();
```

---

Ready to go! Your stock management system is now fully operational. 🚀
