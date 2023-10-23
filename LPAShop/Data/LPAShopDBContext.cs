using Microsoft.EntityFrameworkCore;
using LPAShop.NET06.Models;

namespace LPAShop.NET06.Data
{
    public class LPAShopDBContext : DbContext
    {
        public LPAShopDBContext(DbContextOptions<LPAShopDBContext> options) :base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSpec> ProductSpecs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReviewProduct> ReviewProducts { get; set; }
    }
}
