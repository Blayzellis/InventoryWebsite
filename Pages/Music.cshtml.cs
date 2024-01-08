using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryWebsite.Controllers;



namespace InventoryWebsite
{
    public class MusicModel : PageModel
    {
        public void OnGet()
        {
            
        }

        public async Task OnPostAsync(string Url)
        {
            //string Url = "https://www.youtube.com/watch?v=tSLRBe_F9qU&list=PLvR1Vs9Qj4fk-VnGR2xUtNLvLcwBwGB6V&index=4";
            Console.WriteLine(Url);
            //await ConvertController.DownloadMp3(Url);
            
        }

        public void ConvertToDfpwm(string path)
        {

        }
    }
}
