using InventoryWebsite.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using InventoryWebsite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryWebsite.Logic;
using System.Data.Entity.Core.Objects;



namespace InventoryWebsite.Pages;


public class IndexModel : PageModel
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger<IndexModel> logger;

    public IndexModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContext, ILogger<IndexModel> _logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContext = httpContext;
        logger = _logger;
    }

    public bool serverStatus = false;

    public static Dictionary<string, string> _dict = null;

    [BindProperty]
    public static Dictionary<string, string?> data { get; set; }
    [BindProperty]
    public Dictionary<string, List<ChestModel>>? ItemSets { get; set; }
    [BindProperty]
    public List<string>? ChestsOnline { get; set; }
    [BindProperty]
    public string? currentChest { get; set; }

    public async Task OnGetAsync(string chest = "Blayze")
    {
        if(data is null) {
            data = new Dictionary<string, string?>();
        }
        currentChest = chest;
        ChestsOnline = new List<string>();
        GetChests();
        await PageLoad(chest);
    }

    public void GetChests()
    {
        foreach (WebSocketController wsc in WebSocketController.PC)
        {
            ChestsOnline.Add(wsc.name);
            if(!data.ContainsKey(wsc.name)) {
            data.Add(wsc.name, null);
            }
        }
    }

    public async Task PageLoad(string chest)
    {
        logger.LogInformation("HereFinally!");
        WebSocketController _main = null;
        if(!data.ContainsKey(currentChest)) {
            data.Add(currentChest, null);
        }

        foreach (WebSocketController wsc in WebSocketController.PC)
        {
            if (wsc.name == currentChest)
            {
                _main = wsc;
            }
        }
        if (_main is not null)
        {
            if (_main.mode == WebSocketController.Mode.Ping)
            {
                logger.LogInformation("Here2.0");
                data[currentChest] = DoGetTask(_main);
            }
            serverStatus = true;
        }
        else
        {
            serverStatus = false;
        }


        if (data[currentChest] is null)
        {
        }
        else
        {
            ItemSets = SetModel(JsonSerializer.Deserialize<IEnumerable<ChestModel>>(data[currentChest]));
            await GetImage();
        }
    }

    public string DoGetTask(WebSocketController main)
    {
        var task = Task.Run(() =>
        {
            return main.OnGet();
        });

        bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(20));

        if (isCompletedSuccessfully)
        {
            return task.Result;
        }
        else
        {
            throw new TimeoutException("The function has taken longer than the maximum time allowed.");
        }
    }
    public Dictionary<string, List<ChestModel>> SetModel(IEnumerable<ChestModel> Model)
    {
        Dictionary<string, List<ChestModel>> newSet = new Dictionary<string, List<ChestModel>>();
        foreach (ChestModel item in Model)
        {
            if (newSet.ContainsKey(item.name))
            {
                newSet[item.name].Add(item);
            }
            else
            {
                var temp = new List<ChestModel>
                {
                    item
                };
                newSet.Add(item.name, temp);
            }
        }
        return newSet;
    }

    public async Task GetImage()
    {
        bool changes = false;
        _dict ??= await Load(@"/mounts/files/myDirectory/Urls.json");
        if (ItemSets is null)
            return;
        foreach (List<ChestModel> list in ItemSets.Values)
        {
            foreach (ChestModel obj in list)
            {
                if (_dict.ContainsKey(obj.name))
                {
                    obj.Url = _dict[obj.name];
                }
                else
                {
                    changes = true;
                    string? url = await SearchFor(obj.name);
                    obj.Url = url;
                    if (!(url is null))
                        _dict.Add(obj.name, url);
                }
            }
        }
        if (changes)
        {
            await Save(@"/mounts/files/myDirectory/Urls.json", _dict);
        }
    }

    private async Task<string> SearchFor(string name)
    {
        // Create the HTTP client using the FruitAPI named factory
        var httpClient = _httpClientFactory.CreateClient("GoogleImages");
        var queryParameters = new Dictionary<string, string>
            {
                { "cx", "239e90b60c03d4ec6" },
                { "num", "5" },
                { "q", $"minecraft {name.Split(':')[1]}"},
                { "searchType", "image"},
                { "imgColorType", "trans"},
                { "start", "0"},
                { "filter", "0"},
                { "hl", "English"},
                { "key", Environment.GetEnvironmentVariable("GOOGLE_API_KEY") }
            };
        var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
        var req = await dictFormUrlEncoded.ReadAsStringAsync();
        // Perform the GET request and store the response. The empty parameter
        // in GetAsync doesn't modify the base address set in the client factory 
        using HttpResponseMessage response = await httpClient.GetAsync($"v1?{req}");

        // If the request is successful deserialize the results into the data model
        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            var result = JsonDocument.Parse(contentStream);
            string? image = null;
            string howMany = result.RootElement.GetProperty("searchInformation").GetProperty("totalResults").GetString();
            for (int i = 0; i < Int32.Parse(howMany); i++)
            {
                try
                {
                    image = result.RootElement.GetProperty("items")[i]
                        .GetProperty("image").GetProperty("thumbnailLink").GetString();
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine("Ehhhhh forget ahbout ittt");
                }
                if (!(image is null))
                    return image;

            }
            logger.LogInformation(image);
            return image;
        }
        return null;
    }



    // A method to save the dictionary to disk
    public static async Task Save(string path, Dictionary<string, string> _dictionary)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync(fs, _dictionary);
        }
    }

    // A method to load the dictionary from the disk
    private async Task<Dictionary<string, string>> Load(string path)
    {
        Dictionary<string, string>? _dictionary = null;
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            _dictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(fs);
        }
        _dictionary ??= new Dictionary<string, string>();
        return _dictionary;
    }
}
