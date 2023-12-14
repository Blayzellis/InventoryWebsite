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
        await PageLoad();
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

    public async Task PageLoad()
    {
        //logger.LogInformation("HereFinally!");
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
            if (_main.mode == WebSocketController.Mode.Ping || _main.mode == WebSocketController.Mode.Post)
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
                    if(url is null) {
                        url = "data:image/png;base64,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";
                    }
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
