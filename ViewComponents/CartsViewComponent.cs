using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InventoryWebsite.Models;
using InventoryWebsite.Logic;
namespace InventoryWebsite.ViewComponents
{
    public class CartsViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContext;
        public CartsViewComponent(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = GetShoppingCartItems();
            return View(items);
        }

        public List<CartItem> GetShoppingCartItems()
        {
            ShoppingCartActions actions = new ShoppingCartActions(_httpContext);
            return actions.GetCartItems();
        }

    }
}