using Microsoft.EntityFrameworkCore;
using EcommerceRazorApp.Models;

namespace EcommerceRazorApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Category-Product relationship (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order-OrderItem relationship (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order-Payment relationship (One-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { CategoryId = 2, Name = "Books", Description = "Books and educational materials" },
                new Category { CategoryId = 3, Name = "Clothing", Description = "Fashion and apparel" },
                new Category { CategoryId = 4, Name = "Home", Description = "Home decor and essentials" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Wireless Headphones",
                    Description = "Premium wireless headphones with noise cancellation and 30-hour battery life",
                    Price = 149.99m,
                    Stock = 50,
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&h=500&fit=crop",
                    IsFeatured = true,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Bluetooth Speaker",
                    Description = "Portable waterproof Bluetooth speaker with powerful bass",
                    Price = 79.99m,
                    Stock = 75,
                    ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500&h=500&fit=crop",
                    IsFeatured = true,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Clean Code - Robert Martin",
                    Description = "A handbook of agile software craftsmanship for professional developers",
                    Price = 42.99m,
                    Stock = 100,
                    ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=500&h=500&fit=crop",
                    IsFeatured = false,
                    CategoryId = 2
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Cotton T-Shirt",
                    Description = "Comfortable 100% organic cotton t-shirt in multiple colors",
                    Price = 24.99m,
                    Stock = 200,
                    ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&h=500&fit=crop",
                    IsFeatured = true,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 5,
                    Name = "Winter Jacket",
                    Description = "Warm and stylish winter jacket with premium insulation",
                    Price = 129.99m,
                    Stock = 60,
                    ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=500&h=500&fit=crop",
                    IsFeatured = false,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 6,
                    Name = "Ceramic Vase",
                    Description = "Elegant handcrafted ceramic vase for home decoration",
                    Price = 34.99m,
                    Stock = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1578500494198-246f612d3b3d?w=500&h=500&fit=crop",
                    IsFeatured = true,
                    CategoryId = 4
                },
                new Product
                {
                    ProductId = 7,
                    Name = "USB-C Fast Charger",
                    Description = "65W fast charging adapter with USB-C cable included",
                    Price = 29.99m,
                    Stock = 150,
                    ImageUrl = "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?w=500&h=500&fit=crop",
                    IsFeatured = false,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 8,
                    Name = "Leather Notebook",
                    Description = "Premium leather-bound notebook with 200 pages",
                    Price = 19.99m,
                    Stock = 80,
                    ImageUrl = "https://images.unsplash.com/photo-1544816155-12df9643f363?w=500&h=500&fit=crop",
                    IsFeatured = false,
                    CategoryId = 2
                }
            );
        }
    }
}
