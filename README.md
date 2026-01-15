# ASP.NET Core E‑Commerce Website & Admin Panel

A complete e‑commerce solution built with ASP.NET Core, Entity Framework Core, and a clean n‑tier architecture.  
The solution contains:
- A **Razor Pages storefront** for customers  
- A **separate MVC Admin Panel** for managing products, categories, orders, and payments  
- Shared **Domain / Application / Infrastructure** projects following **repository pattern** and **service layer**.

---

## Solution Overview

### Projects in the Solution

- **EcommerceRazorApp**  
  - Customer‑facing e‑commerce website (Razor Pages)
  - Uses `ApplicationDbContext` for store data (products, orders, payments)

- **Ecommerce.Admin**  
  - ASP.NET Core MVC Admin Panel
  - Uses `AppDbContext` (from Infrastructure) and shared domain entities
  - Provides CRUD for Product, Category and basic order/payment management

- **Ecommerce.Domain**  
  - Pure domain entities:
    - `Product`, `Category`, `Order`, `OrderItem`, `Payment`
  - Contains only data models and validation attributes

- **Ecommerce.Application**  
  - Application layer (business logic and contracts)
  - Interfaces:
    - `IRepository<T>`, `IProductRepository`, `ICategoryRepository`
    - `IProductService`, `IDashboardService`
  - Services:
    - `ProductService` (business logic for products)
    - `DashboardService` (aggregated metrics for admin dashboard)

- **Ecommerce.Infrastructure**  
  - Data access layer (EF Core + repositories)
  - `AppDbContext` (Admin DB context)
  - Generic repository `Repository<T>`
  - Concrete repositories: `ProductRepository`, `CategoryRepository`
  - EF Core migrations for `AppDbContext`

This structure satisfies a clean **n‑tier architecture**:
- Presentation: `EcommerceRazorApp`, `Ecommerce.Admin`
- Application: `Ecommerce.Application`
- Domain: `Ecommerce.Domain`
- Infrastructure/Data: `Ecommerce.Infrastructure`

---

## Storefront Features (EcommerceRazorApp)

### Core Functionality
- **Home Page** – Hero section with featured products
- **Products Page** – Browse all products with:
  - Category filtering
  - Price sorting
  - Pagination
- **Product Details** – Full product info with Add to Cart / Buy Now
- **Shopping Cart** – Session‑based cart (add, remove, update quantity)
- **Checkout** – Customer information + payment selection
- **Invoice** – Order confirmation with customer details, line items, totals

### Payment Methods
- **Dummy Visa Card**
  - Card numbers starting with `4` → successful
  - Others → failed
- **Cash On Delivery**
  - No card required
  - Payment status initially **Pending**

### Storefront Technical Highlights
- ASP.NET Core Razor Pages
- Entity Framework Core Code‑First with migrations
- Dependency Injection with service interfaces
- Session‑based shopping cart
- Bootstrap 5 responsive UI
- Structured error logging with `ILogger`

---

## Admin Panel Features (Ecommerce.Admin)

The admin panel is a separate ASP.NET Core MVC app that uses the shared Domain/Application/Infrastructure layers.

### Dashboard
- Modern dark dashboard layout
- Overview metrics:
  - Total Products
  - Total Categories
  - Total Orders
  - Total Revenue
  - Today’s orders
  - Low stock products count
- Quick shortcuts:
  - View recent orders
  - Manage products
  - Manage categories

### Category Management (Full CRUD)
- List all categories
- Create new category
- Edit existing category
- View category details
- Delete category

Controller: `CategoryController` using `ICategoryRepository`  
Views: `Ecommerce.Admin/Views/Category/Index, Create, Edit, Details, Delete`

### Product Management (Full CRUD)
- List all products including category
- Create product:
  - Name, description, price, stock
  - Category selection
  - Image URL
  - Featured flag
- Edit product
- View details
- Delete product

Implementation:
- `ProductController` (MVC)
- `IProductService` / `ProductService` in Application layer
- `IProductRepository` / `ProductRepository` in Infrastructure

Business rules in `ProductService`:
- Valid category required
- Price and stock must be non‑negative
- Uses repository + `SaveChangesAsync` to persist data

### Orders & Payment Management

- **Orders list**
  - Recent orders ordered by date
  - Quick access to details

- **Order details**
  - Order summary (customer, totals, shipping address)
  - Line items table
  - Order status dropdown (Pending, Processing, Shipped, Delivered, Cancelled)
  - Payment info (method, status)

- **Status updates**
  - Update order status (via `UpdateStatus` action)
  - Update payment status:
    - Allowed values: Pending, Completed, Failed, Refunded
    - Validated and saved using `Payment` repository

---

## Architecture & Patterns

### N‑Tier + Repository Pattern

- **Domain layer** (`Ecommerce.Domain`)
  - Plain C# entity classes with data annotations

- **Application layer** (`Ecommerce.Application`)
  - Contains use‑case oriented services
  - Works with repositories via interfaces (`IRepository<T>`, `IProductRepository`, `ICategoryRepository`)

- **Infrastructure layer** (`Ecommerce.Infrastructure`)
  - Implements repositories using EF Core:
    - Generic `Repository<T>` with:
      - `GetAllAsync`, `GetByIdAsync`, `FindAsync`
      - `AddAsync`, `UpdateAsync`, `RemoveAsync`, `SaveChangesAsync`
    - `ProductRepository` with `GetAllWithCategory`, `GetByIdWithCategory`, `GetByCategory`

- **Presentation layers**
  - Razor Pages storefront (`EcommerceRazorApp`)
  - MVC Admin Panel (`Ecommerce.Admin`)

### Dependency Injection

**Admin Panel** (`Ecommerce.Admin/Program.cs`):

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
```

Storefront app also registers its own services for products, cart, orders, and payments.

---

## Database & Entity Framework

### SQL Server Configuration

Connection string (Admin & Storefront use the same database name):

```text
Server=localhost\SQLEXPRESS;
Database=ecommerce_site;
Trusted_Connection=True;
TrustServerCertificate=True;
```

### Contexts

- `ApplicationDbContext` (storefront)
  - Used by Razor Pages app for main e‑commerce flow
- `AppDbContext` (admin)
  - Defined in `Ecommerce.Infrastructure`
  - Exposes DbSets:
    - `Categories`, `Products`, `Orders`, `OrderItems`, `Payments`

### Relationships

- Category 1‑* Products
- Order 1‑* OrderItems
- Order 1‑1 Payment

### Migrations

- Storefront migrations: `Migrations/` (root)
- Admin context migrations:
  - `Ecommerce.Infrastructure/Ecommerce.Infrastructure/Migrations`
- Design‑time factory:
  - `DesignTimeDbContextFactory` reads `DefaultConnection` from:
    - root `appsettings.json`
    - `Ecommerce.Admin/appsettings.json`

You can run migrations (example for Infrastructure context):

```bash
dotnet ef database update -p Ecommerce.Infrastructure -s Ecommerce.Admin
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server / SQL Server Express
- Visual Studio 2022 or VS Code

---

## How to Run

### 1. Restore packages

```bash
dotnet restore
```

### 2. Update connection string (if needed)

In `Ecommerce.Admin/appsettings.json` and/or root `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ecommerce_site;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply migrations (optional manual step)

If you want to ensure database is created/updated manually:

```bash
dotnet tool install --global dotnet-ef

dotnet ef database update -p Ecommerce.Infrastructure -s Ecommerce.Admin
```

### 4. Run the storefront

From solution root:

```bash
dotnet run --project EcommerceRazorApp/EcommerceRazorApp.csproj
```

### 5. Run the admin panel

From solution root:

```bash
dotnet run --project Ecommerce.Admin/Ecommerce.Admin.csproj
```

Admin panel URLs (from launchSettings of `Ecommerce.Admin`):
- HTTP: `http://localhost:5107`
- HTTPS: `https://localhost:7070`

Actual ports may vary; check console output.

---

## Storefront Usage Flow

1. Browse products from Home or Products page
2. Filter/sort products by category and price
3. View product details
4. Add items to cart or buy now
5. Review cart and adjust quantities
6. Checkout with customer details and payment method
7. View invoice with full order summary

### Testing Payments

- **Successful Visa** – any card starting with `4` (e.g. `4111 1111 1111 1111`)
- **Failed Visa** – any card not starting with `4` (e.g. `5555 5555 5555 5555`)
- **Cash On Delivery** – choose COD; payment remains `Pending`

---

## Admin Panel Usage Flow

1. Open Admin panel (`/` of `Ecommerce.Admin`)
2. Overview dashboard shows totals and quick stats
3. Use sidebar navigation:
   - **Products** – full CRUD on products
   - **Categories** – full CRUD on categories
   - **Orders** – view recent orders and drill into details
4. In order details:
   - Update order status from dropdown
   - Update payment status (Pending/Completed/Failed/Refunded)

---

## Technologies

- **Framework:** ASP.NET Core 9 (Razor Pages + MVC)
- **ORM:** Entity Framework Core 9
- **Database:** SQL Server / SQL Server Express
- **Architecture:** N‑tier (Domain / Application / Infrastructure / Presentation)
- **Patterns:** Repository pattern, Service layer, Dependency Injection
- **UI:** Bootstrap 5, custom dark admin layout
- **Session:** In‑memory session store for cart

---

## Security & Best Practices

- Model validation on all forms
- EF Core parameterization to avoid SQL injection
- HTTPS enabled in development profiles
- Error details hidden from end users in production
- Admin panel separated from storefront

---

## Support

If you run into issues:
1. Verify .NET SDK is installed (`dotnet --version`)
2. Ensure SQL Server is running and reachable
3. Check `ConnectionStrings:DefaultConnection`
4. Apply EF migrations as needed
5. Review application console logs for detailed errors

---

**Built with ASP.NET Core | Entity Framework Core | Bootstrap 5 | Clean Architecture**
