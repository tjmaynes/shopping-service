using Microsoft.EntityFrameworkCore;
using ShoppingService.Core.Cart;

namespace ShoppingService.Api {
    public class ApplicationContext: DbContext
    {
        public DbSet<CartItem> CartItems { get; set; }
    }
}