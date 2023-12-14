using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InventoryWebsite.Models;

namespace InventoryWebsite.Logic
{
  public class ShoppingCartActions : IDisposable
  {
    public string ShoppingCartId { get; set; }

    private ProductContext _db = new ProductContext();
    private readonly IHttpContextAccessor _contextAccessor;

    public ShoppingCartActions(IHttpContextAccessor contextAccessor)
    {
      _contextAccessor = contextAccessor;
    }


    public const string CartSessionKey = "CartId";

    public void AddToCart(string id, bool direction, string origin = "main")
    {
      // Retrieve the product from the database.           
      ShoppingCartId = GetCartId();
      Console.WriteLine(ShoppingCartId);

      var cartItem = _db.ShoppingCartItems.SingleOrDefault(
          c => c.CartId == ShoppingCartId
          && c.ProductId == id);
      if (cartItem == null)
      {
        // Create a new cart item if no cart item exists.                 
        cartItem = new CartItem
        {
          ItemId = Guid.NewGuid().ToString(),
          CartId = ShoppingCartId,
          //Product = _db.Products.SingleOrDefault(
          //p => p.ProductID == id),
          Quantity = 1,
          ProductId = id,
          Origin = origin,
          Direction = direction
        };

        _db.ShoppingCartItems.Add(cartItem);
      }
      else
      {
        // If the item does exist in the cart,                  
        // then add one to the quantity.                 
        cartItem.Quantity++;
      }
      _db.SaveChanges();
    }

    public void Dispose()
    {
      if (_db != null)
      {
        _db.Dispose();
        _db = null;
      }
    }

    public string GetCartId()
    {
      var Response = _contextAccessor.HttpContext.Response;
      var Request = _contextAccessor.HttpContext.Request;
      var cookieValue = Request.Cookies["UserId"];
      var cookieOptions = new CookieOptions
      {
        Expires = DateTime.Now.AddMinutes(10)
      };
      if (cookieValue is null)
      {

        if (!string.IsNullOrWhiteSpace(_contextAccessor.HttpContext.User.Identity.Name))
        {
          Response.Cookies.Append("UserId", _contextAccessor.HttpContext.User.Identity.Name, cookieOptions);
        }
        else
        {
          // Generate a new random GUID using System.Guid class.     
          Guid tempCartId = Guid.NewGuid();
          Response.Cookies.Append("UserId", tempCartId.ToString(), cookieOptions);
        }
      }
      else
      {
        Response.Cookies.Append("UserId", cookieValue, cookieOptions);
      }
      return Request.Cookies["UserId"];
    }

    public List<CartItem> GetCartItems()
    {
      ShoppingCartId = GetCartId();

      return _db.ShoppingCartItems.Where(
          c => c.CartId == ShoppingCartId).ToList();
    }

    public void EmptyCart() {
      ShoppingCartId = GetCartId();
      var deleteList = _db.ShoppingCartItems.Where(
          c => c.CartId == ShoppingCartId).ToList();
      foreach(CartItem cartItem in deleteList) {
        _db.ShoppingCartItems.Remove(cartItem);
      }
      _db.SaveChanges();
    }

    public void UpdateCart(string[] productIds, int[] qty)
    {
      ShoppingCartId = GetCartId();
      int i = 0;
      foreach (string s in productIds)
      {
        var cartItem = _db.ShoppingCartItems.SingleOrDefault(
          c => c.CartId == ShoppingCartId
          && c.ProductId == s);
        if (cartItem is null)
        {
          cartItem = new CartItem
          {
            ItemId = Guid.NewGuid().ToString(),
            CartId = ShoppingCartId,
            //Product = _db.Products.SingleOrDefault(
            //p => p.ProductID == id),
            Quantity = qty[i],
            ProductId = s
          };
          _db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
          cartItem.Quantity = qty[i];
        }
        i++;
      }
      _db.SaveChanges();
    }
  }
}