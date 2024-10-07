using Microsoft.EntityFrameworkCore;
using LPAShop.NET06.Models;

namespace LPAShop.NET06.Data
{
    public class LPAShopDBContext : DbContext
    {
        public LPAShopDBContext(DbContextOptions<LPAShopDBContext> options) :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Order_ID); // Đặt Order_ID làm khóa chính
            });
            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.HasKey(e => e.OrderItem_ID); // Đặt OrderItem_ID làm khóa chính
            });
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSpec> ProductSpecs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReviewProduct> ReviewProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
    }
}
