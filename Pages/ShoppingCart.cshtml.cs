using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryWebsite.Models;
using InventoryWebsite.Logic;
using InventoryWebsite.Controllers;
using System.Text.Json;



namespace InventoryWebsite.Pages
{
    public class Data
    {
        public string Player { get; set; }
        public List<Item> Items { get; set; }
        public Data(string name)
        {
            Player = name;
            Items = new List<Item>();
        }
    }
    public class Item
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public Item(string name, int quantity)
        {
            Name = name;
            Qty = quantity;
        }
    }
    public class ShoppingCartModel : PageModel
    {

        private readonly IHttpContextAccessor _httpContext;
        public static bool serverStatus = true;
        public ShoppingCartModel(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        [BindProperty]
        public List<CartItem>? MyCart { get; set; }
        public void OnGet()
        {
            MyCart = GetShoppingCartItems();
        }

        public List<CartItem> GetShoppingCartItems()
        {
            ShoppingCartActions actions = new ShoppingCartActions(_httpContext);
            return actions.GetCartItems();
        }
        public void ClearCart()
        {
            ShoppingCartActions actions = new ShoppingCartActions(_httpContext);
            actions.EmptyCart();
        }
        public async Task<IActionResult> OnPost(string player = "Blayze")
        {
            MyCart = GetShoppingCartItems();
            Dictionary<string, Data> dict = new Dictionary<string, Data>();
            int count = 0;
            string lastOrigin = "";
            foreach (CartItem item in MyCart)
            {
                Item temp = new Item(item.ProductId, item.Quantity);
                count += temp.Qty;
                if(dict.ContainsKey(item.Origin)) {
                    dict[item.Origin].Items.Add(temp);
                }
                else {
                    var data = new Data(player);
                    data.Items.Add(temp);
                    dict[item.Origin] = data;
                    lastOrigin = item.Origin;
                }
            }
            WebSocketController._main.SendMessage($"Sent {count} item/s to {player}");
            List<Task> tasks = new List<Task>();
            foreach(KeyValuePair<string, Data> d in dict) {
                tasks.Add(SendRequest(d.Value, d.Key));
            }
            await Task.WhenAll(tasks);
            ClearCart();
            return Redirect($"Index?chest={lastOrigin}");
        }
        public async Task SendRequest(Data items, string origin)
        {
            WebSocketController _main = null;

            foreach (WebSocketController wsc in WebSocketController.PC)
            {
                if (wsc.name == origin)
                {
                    _main = wsc;
                }
            }
            if (_main is not null)
            {
                if (_main.mode == WebSocketController.Mode.Ping)
                {
                    var jsonContent = JsonSerializer.Serialize(items);
                    Console.WriteLine($"Post2.0 {jsonContent}");
                    _main.OnPost(jsonContent);
                }
                serverStatus = true;
            }
            else
            {
                serverStatus = false;
            }
        }

    }
}
