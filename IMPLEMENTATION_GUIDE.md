# Dynamic Hero Background Image Implementation Guide

## Overview
The hero background image on the home page is now dynamic and can be changed from the admin dashboard.

## What Was Implemented

### 1. Database Changes
- **SiteSettings Model**: Created a new entity to store site configuration settings
  - File: `DAJDAJ.Entities/Models/SiteSettings.cs`
  - Properties: Id, SettingKey, SettingValue, Description

- **Database Context**: Added SiteSettings DbSet to ApplicationDbContext
  - File: `DAJDAJ.DataAccess/Data/ApplicationDbContext.cs`

### 2. Repository Layer
- **ISiteSettingsRepository**: Interface for site settings operations
  - File: `DAJDAJ.Entities/Repositories/ISiteSettingsRepository.cs`
  
- **SiteSettingsRepository**: Implementation of the repository
  - File: `DAJDAJ.DataAccess/Implementation/SiteSettingsRepository.cs`
  - Methods: Add, Update, GetByKey, GetAll, Delete

- **Unit of Work**: Added SiteSettings repository to the unit of work pattern
  - Files: `DAJDAJ.Entities/Repositories/IUntiOfWork.cs`, `DAJDAJ.DataAccess/Implementation/UnitOfWork.cs`

### 3. Admin Controller
- **SiteSettingsController**: Manages site settings from admin dashboard
  - File: `DAJDAJ.Web/Areas/Admin/Controllers/SiteSettingsController.cs`
  - Actions:
    - `Index()`: Lists all site settings
    - `EditHeroImage()`: GET - Displays the form to edit hero image
    - `EditHeroImage(POST)`: Handles image upload and updates the database

### 4. Admin Views
- **Index View**: Lists all site settings with preview
  - File: `DAJDAJ.Web/Areas/Admin/Views/SiteSettings/Index.cshtml`

- **EditHeroImage View**: Form to upload and update hero background image
  - File: `DAJDAJ.Web/Areas/Admin/Views/SiteSettings/EditHeroImage.cshtml`
  - Features:
    - Current image preview
    - File upload with instant preview
    - Image validation
    - Responsive design

### 5. Frontend Changes
- **HomeController**: Updated to fetch hero image from database
  - File: `DAJDAJ.Web/Areas/Customer/Controllers/HomeController.cs`
  - Added: `ViewBag.HeroBackgroundImage` with fallback to default image

- **Index View**: Updated to use dynamic background image
  - File: `DAJDAJ.Web/Areas/Customer/Views/Home/Index.cshtml`
  - Changed: `background-image: url('@ViewBag.HeroBackgroundImage')`

- **Dashboard Layout**: Added "Site Settings" menu item
  - File: `DAJDAJ.Web/Views/Shared/_Dashboard.cshtml`

## How to Use

### Step 1: Run Database Migration
Execute the following commands in the terminal:

```bash
cd "C:\Users\youss\source\repos\DAJDAJ"
dotnet ef migrations add AddSiteSettings --project DAJDAJ.DataAccess --startup-project DAJDAJ.Web
dotnet ef database update --project DAJDAJ.DataAccess --startup-project DAJDAJ.Web
```

### Step 2: Access Admin Dashboard
1. Log in to your application as an Admin
2. Navigate to the Admin Dashboard
3. Click on "Site Settings" in the sidebar menu

### Step 3: Change Hero Background Image
1. Click on "Edit" button next to "HeroBackgroundImage" setting
   - Or if no settings exist, click "Add Hero Image"
2. Upload a new image (recommended size: 1920x1080 pixels or 16:9 aspect ratio)
3. Add/edit the description if needed
4. Click "Save Changes"
5. The home page hero section will now display your new image

## Features

### Image Upload
- Automatic file naming with GUID to prevent conflicts
- Old image deletion (keeps the default image safe)
- Instant preview before saving
- File type validation (images only)

### Fallback Mechanism
- If no custom image is set, uses the default image: `/Images/IMG-20250724-WA0059.jpg`
- Ensures the site always has a hero image

### Admin Interface
- Clean, professional UI matching the AdminLTE theme
- Real-time image preview
- Responsive design for mobile/desktop
- Success/error notifications with Toastr

## File Structure

```
DAJDAJ.Entities/
??? Models/
?   ??? SiteSettings.cs
??? Repositories/
    ??? ISiteSettingsRepository.cs
    ??? IUntiOfWork.cs (modified)

DAJDAJ.DataAccess/
??? Implementation/
?   ??? SiteSettingsRepository.cs
?   ??? UnitOfWork.cs (modified)
??? Data/
    ??? ApplicationDbContext.cs (modified)

DAJDAJ.Web/
??? Areas/
?   ??? Admin/
?   ?   ??? Controllers/
?   ?   ?   ??? SiteSettingsController.cs
?   ?   ??? Views/
?   ?       ??? SiteSettings/
?   ?           ??? Index.cshtml
?   ?           ??? EditHeroImage.cshtml
?   ??? Customer/
?       ??? Controllers/
?       ?   ??? HomeController.cs (modified)
?       ??? Views/
?           ??? Home/
?               ??? Index.cshtml (modified)
??? Views/
    ??? Shared/
        ??? _Dashboard.cshtml (modified)
```

## Benefits

1. **No Code Changes Required**: Admins can change the hero image without touching code
2. **Image Management**: Automatic handling of image uploads and deletions
3. **Extensible**: Easy to add more site settings in the future
4. **User-Friendly**: Simple interface with instant preview
5. **Safe**: Fallback mechanism ensures site always works

## Future Enhancements

You can easily extend this system to manage other site settings:
- Site title and tagline
- Social media links
- Footer text
- Contact information
- Theme colors
- Logo images
- Featured products

Simply add new entries to the SiteSettings table with appropriate keys and implement the UI in the admin dashboard.
