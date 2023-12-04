using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryWebsite.Models;

public class ChestModel
{
    
    public int slot { get; set; }
    [Key]
    public string? name { get; set; }
    [Display(Name ="1-64")]
    public int count { get; set; }
    public string? Url { get; set; }
}
