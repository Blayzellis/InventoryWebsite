using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryWebsite.Pages;

public class ItemModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public ItemModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

