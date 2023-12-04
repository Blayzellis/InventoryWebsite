using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using InventoryWebsite.Pages;

namespace InventoryWebsite.Controllers;

#region snippet_Controller_Connect
public class WebSocketController : ControllerBase
{
    public enum Mode
    {
        Ping,
        Get,
        Busy,
        Post,
        Closed
    }

    public Mode mode = Mode.Ping;
    public string name = "";
    public string? result = null;
    public string? payload = null;
    public static List<WebSocketController> PC = new List<WebSocketController>();
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            PC.Add(this);
            mode = Mode.Ping;
            await UseSocket(webSocket);
            PC.Remove(this);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    #endregion
    public string OnGet() 
    {
        mode = Mode.Get;
        while(mode == Mode.Get){}
        return result;
    }
    public bool OnPost(string data) {
        while(mode != Mode.Ping){}
        mode = Mode.Post;
        payload = data;
        return true;
    }
    private async Task UseSocket(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 48];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        name = System.Text.Encoding.Default.GetString(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count));

        while (!receiveResult.CloseStatus.HasValue && mode != Mode.Closed)
        {
            if (mode == Mode.Ping || mode == Mode.Busy)
            {
                receiveResult = await PingMode(webSocket, receiveResult, buffer);
            }
            else if(mode == Mode.Get) 
            {
                receiveResult = await PingMode(webSocket, receiveResult, buffer);
                result = System.Text.Encoding.Default.GetString(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count));
                mode = Mode.Ping;
            }
            else if(mode == Mode.Post) 
            {
                receiveResult = await PingMode(webSocket, receiveResult, buffer);
                mode = Mode.Ping;
            }

            

            //Console.WriteLine(result);
        }
        mode = Mode.Busy;
        if(receiveResult.CloseStatus is null)
            return;
        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    private async Task<WebSocketReceiveResult> PingMode(WebSocket webSocket, WebSocketReceiveResult receiveResult, byte[] buffer)
    {
        try {
            var message = BitConverter.GetBytes((int) mode);
            if(payload != null) {
                message = Encoding.ASCII.GetBytes(payload);
                payload = null;
            }
            await webSocket.SendAsync(
                new ArraySegment<byte>(message, 0, message.Length), 0 , receiveResult.EndOfMessage, CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch(WebSocketException ex){
            Console.WriteLine($"Handshake Bs {ex.Message}");
            mode = Mode.Closed;
        }

        return receiveResult;
    }
}
