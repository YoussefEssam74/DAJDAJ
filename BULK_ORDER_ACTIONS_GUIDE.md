# Bulk Order Actions - User Guide

## Overview
This feature allows you to filter orders and perform bulk actions on all filtered results without selecting them one by one.

## Features Implemented

### 1. **Filter Orders**
You can filter orders by:
- **Order Status**: Pending, Processing, Booked, Shipped, Earned, Return, Cancelled
- **Start Date**: Orders from this date onwards
- **End Date**: Orders up to this date
- **Customer Name**: Search by customer name
- **Phone**: Search by phone number

### 2. **Bulk Actions on Filtered Orders**

#### **Change All Status**
- Select a new status from the "Change Status To" dropdown
- Click "Change All Status" button
- All filtered orders will be changed to the selected status
- Example: Filter all "Pending" orders, then change them all to "Booked"

#### **Print Filtered Orders**
- After applying filters, click "Print Filtered Orders"
- Opens a new window with all filtered orders
- Each order appears on its own page (page break between orders)
- Perfect for printing multiple orders at once

#### **Export Filtered to Excel**
- Exports only the filtered orders to Excel
- File includes: ID, Name, Phone, Address, Status, Total Price

#### **Mark All as Completed**
- Changes all filtered orders to "Earned" status
- Useful for bulk completion

## How to Use

### Example Workflow 1: Change Pending Orders to Booked
1. Set "Order Status" filter to "Pending"
2. Click "Apply Filters"
3. Select "Booked" from "Change Status To" dropdown
4. Click "Change All Status"
5. Confirm the action
6. All pending orders are now booked!

### Example Workflow 2: Print All Booked Orders
1. Set "Order Status" filter to "Booked"
2. Click "Apply Filters"
3. Click "Print Filtered Orders"
4. A new window opens with all booked orders
5. Each order is on a separate page
6. Use browser print function (Ctrl+P)

### Example Workflow 3: Export Orders by Date Range
1. Set "Start Date" and "End Date"
2. Click "Apply Filters"
3. Click "Export Filtered to Excel"
4. Excel file downloads with filtered orders

## Benefits
- **Save Time**: No need to open each order individually
- **Bulk Operations**: Change hundreds of orders at once
- **Flexible Filtering**: Combine multiple filters
- **Print Multiple**: Print all filtered orders with page breaks
- **Easy Export**: Export filtered data to Excel

## Technical Details

### New Files Created
- `OrderFilterVM.cs` - Filter view model
- `PrintFiltered.cshtml` - Bulk print view

### Modified Files
- `OrderController.cs` - Added filter and bulk action methods
- `Index.cshtml` - Added filter UI and bulk action buttons
- `Order.js` - Added JavaScript for filters and bulk actions

### New Controller Actions
- `GetData(filters)` - Returns filtered orders
- `ChangeAllStatus(newStatus, filters)` - Changes all filtered orders status
- `PrintFiltered(filters)` - Returns print view for filtered orders
- `ExportFilteredToExcel(filters)` - Exports filtered orders to Excel
- `MarkAllAsCompleted(filters)` - Marks filtered orders as completed

## Notes
- Print view has page breaks between orders for clean printing
- Confirmation dialogs prevent accidental bulk changes
- All actions respect the current filters
- Success messages show how many orders were affected
