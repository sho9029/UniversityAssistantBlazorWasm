using Append.Blazor.Notifications;
using Blazored.SessionStorage;
using Blazored.LocalStorage;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UniversityAssistantBlazorWasm.Tools;

namespace UniversityAssistantBlazorWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddOptions();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazorFluentUI();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddNotifications();
            builder.Services.AddAuthentication();
            builder.Services.AddFirebaseAuth();
            await builder.Build().RunAsync();
        }
    }
}
