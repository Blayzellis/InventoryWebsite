//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
 
namespace InventoryWebsite.Models
{
    public class ProductContext : DbContext
    {
        protected override void OnConfiguring
       (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "Cart");
        }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
    }
}