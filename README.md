# ASP.NET Core Razor Pages E-Commerce Website

A fully functional e-commerce website built with ASP.NET Core Razor Pages, Entity Framework Core (Code-First), and industry best practices.

## Features

### Core Functionality
- **Home Page** - Hero section with featured products
- **Products Page** - Browse all products with category filtering, price sorting, and pagination
- **Product Details** - View detailed product information with Add to Cart and Buy Now
- **Shopping Cart** - Session-based cart with add, remove, and update quantity
- **Checkout** - Customer information form with payment options
- **Invoice** - Order confirmation with customer info, items, and payment details

### Payment Methods
- **Dummy Visa Card** - Cards starting with 4 = successful, others = failed
- **Cash On Delivery** - Payment status set to Pending

### Technical Features
- Entity Framework Core Code-First with Migrations
- Dependency Injection with IService pattern
- Session management for shopping cart
- Comprehensive error handling with ILogger
- Bootstrap 5 responsive design
- Database seeding with sample data

## Architecture

### Folder Structure
```
/Data
  ApplicationDbContext.cs

/Models
  Product.cs
  Category.cs
  CartItem.cs
  Order.cs
  OrderItem.cs
  Payment.cs
  CustomerInfo.cs

/Services
  /Interfaces
    IProductService.cs
    ICartService.cs
    IOrderService.cs
    IPaymentService.cs
  /Implementations
    ProductService.cs
    CartService.cs
    OrderService.cs
    PaymentService.cs

/Pages
  Index.cshtml (Home)
  /Products
    Index.cshtml (All Products)
    Details.cshtml (Product Details)
  /Cart
    Index.cshtml (Shopping Cart)
  /Checkout
    Index.cshtml (Checkout)
  /Invoice
    Index.cshtml (Invoice/Receipt)
  /Shared
    _Layout.cshtml
    _ValidationScriptsPartial.cshtml

/wwwroot
  /css
  /js
```

## Database Configuration

### Connection String
```
Server=localhost\SQLEXPRESS;
Database=ecommerce_site;
Trusted_Connection=True;
TrustServerCertificate=True;
```

### Database Relationships
- **Category → Products** (One-to-Many) - Cascade Delete
- **Order → OrderItems** (One-to-Many) - Cascade Delete
- **Order → Payment** (One-to-One) - Cascade Delete

### Seed Data
**Categories:**
1. Electronics
2. Books
3. Clothing
4. Home

**Products:**
1. Wireless Headphones (Featured)
2. Bluetooth Speaker (Featured)
3. Clean Code Book
4. Cotton T-Shirt (Featured)
5. Winter Jacket
6. Ceramic Vase (Featured)
7. USB-C Fast Charger
8. Leather Notebook

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## Setup Instructions

### Step 1: Install .NET 8.0 SDK
Download and install .NET 8.0 SDK from the official Microsoft website.

Verify installation:
```bash
dotnet --version
```

### Step 2: Install SQL Server Express
1. Download SQL Server Express from Microsoft
2. Install with default settings
3. Remember the instance name (usually `localhost\SQLEXPRESS`)

### Step 3: Clone or Download the Project
Navigate to the project directory:
```bash
cd "D:\08. Projects\E-commerce"
```

### Step 4: Restore NuGet Packages
```bash
dotnet restore
```

### Step 5: Update Database Connection String (If Needed)
If your SQL Server instance name is different, update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ecommerce_site;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Step 6: Apply Database Migrations
The application will automatically create the database and apply migrations on first run.

Alternatively, you can manually run:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Note:** If `dotnet ef` is not recognized, install it globally:
```bash
dotnet tool install --global dotnet-ef
```

### Step 7: Run the Application
```bash
dotnet run
```

Or press `F5` in Visual Studio.

### Step 8: Access the Application
Open your browser and navigate to:
- **HTTPS:** https://localhost:7000
- **HTTP:** http://localhost:5000

(Port numbers may vary - check console output)

## Usage Guide

### Shopping Flow
1. **Browse Products** - Visit the home page to see featured products or go to Products page
2. **Filter & Sort** - Use category filter and price sorting on Products page
3. **View Details** - Click on any product to see full details
4. **Add to Cart** - Click "Add to Cart" or "Buy Now"
5. **Review Cart** - View cart, update quantities, or remove items
6. **Checkout** - Fill in customer information and select payment method
7. **Complete Order** - View invoice with order details

### Testing Payment
**Successful Payment:**
- Use any card number starting with 4
- Example: `4111 1111 1111 1111`

**Failed Payment:**
- Use any card number NOT starting with 4
- Example: `5555 5555 5555 5555`

**Cash On Delivery:**
- Select COD option (no card required)
- Payment status will be "Pending"

## Service Layer Architecture

All business logic is in service classes with proper error handling:

```csharp
// Example: ProductService
public async Task<Product?> GetProductByIdAsync(int id)
{
    try
    {
        _logger.LogInformation("Fetching product {ProductId}", id);
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching product {ProductId}", id);
        throw;
    }
}
```

## Dependency Injection Configuration

All services are registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
```

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check instance name in connection string
- Ensure Windows Authentication is enabled

### Migration Errors
```bash
# Remove existing migrations
dotnet ef migrations remove

# Create new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### Port Already in Use
Change ports in `Properties/launchSettings.json` or use:
```bash
dotnet run --urls "https://localhost:5001;http://localhost:5000"
```

### Session Not Working
Ensure session middleware is configured in correct order in `Program.cs`:
```csharp
app.UseRouting();
app.UseSession();  // Must be after UseRouting
app.UseAuthorization();
```

## Project Technologies

- **Framework:** ASP.NET Core 8.0
- **UI:** Razor Pages with Bootstrap 5
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server / SQL Server Express
- **Architecture:** Repository Pattern with Service Layer
- **Design Pattern:** Dependency Injection
- **Session Management:** In-Memory Session Store

## API Endpoints (Razor Pages)

- `GET /` - Home page with featured products
- `GET /Products` - All products with filtering
- `GET /Products/Details?id={id}` - Product details
- `GET /Cart` - Shopping cart
- `GET /Checkout` - Checkout form
- `GET /Invoice?orderId={id}` - Order invoice
- `POST /Products?handler=AddToCart` - Add product to cart
- `POST /Cart?handler=UpdateQuantity` - Update cart quantity
- `POST /Checkout` - Process order

## Security Features

- ModelState validation on all forms
- SQL injection prevention via EF Core parameterization
- Session security with HttpOnly cookies
- HTTPS enforcement in production
- Error details hidden from users in production

## Future Enhancements (Optional)

- User authentication and authorization
- Admin panel for product management
- Order tracking and history
- Email notifications
- Payment gateway integration
- Product reviews and ratings
- Stock management
- Coupon/discount system

## License

This project is created for educational purposes.

## Support

For issues or questions, please check:
1. Verify all prerequisites are installed
2. Check database connection string
3. Ensure migrations are applied
4. Review console logs for errors

---

**Built with ASP.NET Core Razor Pages | Entity Framework Core | Bootstrap 5**
