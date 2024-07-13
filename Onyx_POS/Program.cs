using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Onyx_POS.Data;
using Onyx_POS.Services;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddSingleton<AppDbContext>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<CommonService>();
        builder.Services.AddSingleton<SalesService>();
        builder.Services.AddSingleton<LogService>();
        // Add services to the container.
        builder.Services.AddControllersWithViews()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
                {
            new("en-GB"),
            new("ar"),
            new("fa")
                };

            options.DefaultRequestCulture = new RequestCulture("en-GB");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            foreach (var culture in supportedCultures)
                culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            options.RequestCultureProviders = [new QueryStringRequestCultureProvider(), new CookieRequestCultureProvider()];
        });
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        builder.WebHost.UseElectron(args);
        builder.Services.AddElectron();
        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        var supportedCultures = new[] { "en-GB", "ar", "fa" };
        var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture("en-GB")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.UseWebSockets();
        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var ws = await context.WebSockets.AcceptWebSocketAsync();
                while (true)
                {
                    var isConnected = CheckRemoteConnection();
                    var bytes = Encoding.UTF8.GetBytes(isConnected.ToString());
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    if (ws.State == WebSocketState.Open)
                        await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            else
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        });
        if (HybridSupport.IsElectronActive)
            CreateElectronWindow();
        app.Run();
        static async void CreateElectronWindow()
        {
            var windowOptions = new BrowserWindowOptions
            {
                //Fullscreen = true,
                Resizable = false,
                AutoHideMenuBar = true,
                Show = false,
                Icon = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/media/logo/small-logo.png"),
            };
            var mainWindow = await Electron.WindowManager.CreateWindowAsync(windowOptions);
            mainWindow.SetTitle("Onyx POS Dialog");
            //mainWindow.LoadURL("https://google.com");
            mainWindow.OnReadyToShow += () =>
            {
                mainWindow.Maximize();
                mainWindow.Show();
            };
        }
        bool CheckRemoteConnection()
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var commonService = services.GetRequiredService<CommonService>();
            bool isConnected = commonService.CheckDatabaseConnection();
            return isConnected;
        }
    }
}