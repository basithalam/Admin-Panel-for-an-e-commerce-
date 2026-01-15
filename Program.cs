using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using EcommerceRazorApp.Data;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using EcommerceRazorApp.Services.Interfaces;
using EcommerceRazorApp.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Configure Entity Framework Core with SQL Server (Infrastructure DbContext)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

// Register ApplicationDbContext for storefront services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpContextAccessor for session access
builder.Services.AddHttpContextAccessor();

// Register repositories and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic repository
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Product repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Category repository
builder.Services.AddScoped<Ecommerce.Application.Interfaces.IProductService, Ecommerce.Application.Services.ProductService>(); // New Product service
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Keep existing non-product services for storefront
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<EcommerceRazorApp.Services.Interfaces.IProductService, EcommerceRazorApp.Services.Implementations.ProductService>();

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
