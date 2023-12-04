using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using InventoryWebsite.Logic;

namespace InventoryWebsite.Pages
{
    public class AddToCartModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContext;
        public AddToCartModel(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public void OnGet(string rawId, string origin)
        {
            AddToCart(rawId, origin);
            //return Redirect("Index");
        }
        public async Task<IActionResult> OnPostEditAsync(string[] Id, int[] amount)
        {
            using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions(_httpContext))
            {
                usersShoppingCart.UpdateCart(Id, amount);
            }
            return Redirect("ShoppingCart");
        }

        public void AddToCart(string rawId, string origin)
        {
            int productId;
            if (!String.IsNullOrEmpty(rawId)) //&& int.TryParse(rawId, out productId)
            {
                using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions(_httpContext))
                {
                    usersShoppingCart.AddToCart(rawId, origin); //Convert.ToInt16(rawId)
                }

            }
            else
            {
                Debug.Fail("ERROR : We should never get to AddToCart.aspx without a ProductId.");
                throw new Exception("ERROR : It is illegal to load AddToCart.aspx without setting a ProductId.");
            }
        }
    }
}
