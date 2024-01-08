using InventoryWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging.AzureAppServices;
using FFMpegCore;

//239e90b60c03d4ec6

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<ProductContext>(opt => opt.UseInMemoryDatabase("InventoryWebsite"));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddAzureWebAppDiagnostics();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("GoogleImages", httpClient =>
 {
     httpClient.BaseAddress = new Uri("https://customsearch.googleapis.com/customsearch/");
 });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "/mounts/files/myDirectory/ffmpeg/bin", TemporaryFilesFolder = "/tmp" });
app.Run();
