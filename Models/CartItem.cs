using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryWebsite.Models;

public class CartItem
{
     [Key]
        public string ItemId { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public string? Origin { get; set; } //Player name if from player
        public bool Direction  { get; set; } = true;
        public string ProductId { get; set; }
}
