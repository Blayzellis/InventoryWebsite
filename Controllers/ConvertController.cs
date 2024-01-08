using Microsoft.AspNetCore.Mvc;
using VideoLibrary;
using FFMpegCore;
using FFMpegCore.Pipes;
using FFMpegCore.Enums;
using System.Text;
using InventoryWebsite.Pages;

namespace InventoryWebsite.Controllers;
[Route("Music/convert")]
[ApiController]
public class ConvertController : ControllerBase
{
    public Dictionary<string, Task> downloads = new Dictionary<string, Task>();
    [HttpGet("{Url}")]
    public async Task<ActionResult> GetTodoItem(string Url)
    {
        Console.WriteLine(Url);
        string path = @$"/mounts/files/myDirectory/VideoCache/{Url}.dfpwm";
        bool ongoing = downloads.ContainsKey(Url);
        //Check Task ongoing
        if (ongoing)
        {
            Console.WriteLine("Here");
            if (downloads[Url].IsCompleted)
            {
                downloads.Remove(Url);
            }
            else
            {
                Console.WriteLine("????");
                return NoContent(); //NotDone
            }

        }
        //Check file exists
        if (!System.IO.File.Exists(path))
        {
            Task temp = DownloadMp3(Url);
            downloads.Add(Url, temp);
            return Accepted();
        }
        Stream file = new FileStream(path, FileMode.Open);
        return File(file, "audio/dfpwm", $"{Url}.dfpwm");
    }


    public static async Task DownloadMp3(string url)
    {
        string path = $@"/mounts/files/myDirectory/VideoCache/";
        YouTubeVideo video = null;
        using (var cli = Client.For(new YouTube()))
        {
            var videoInfos = cli.GetAllVideosAsync("https://www.youtube.com/watch?v=" + url).GetAwaiter().GetResult();
            video = videoInfos.First(i => i.Resolution == videoInfos.Min(j => j.Resolution));
            path += url + ".dfpwm"; //video.Title.Trim()
        }
        byte[] bytes = await video.GetBytesAsync();
        MemoryStream stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        await using (var audioOutputStream = System.IO.File.Open(path, FileMode.Create))
        {
            Console.WriteLine("Converting");
            await FFMpegArguments
                .FromPipeInput(new StreamPipeSource(stream))
                .OutputToPipe(new StreamPipeSink(audioOutputStream), options =>
                    options.ForceFormat("dfpwm")
                    .WithAudioBitrate(48)
                    .WithCustomArgument("-ac 1"))
                .ProcessAsynchronously();
        }


        //System.IO.File.WriteAllBytes(path, bytes);
        Console.WriteLine($"Saved at {path}");
    }


}
