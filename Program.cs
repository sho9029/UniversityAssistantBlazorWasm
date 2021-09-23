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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Blazored.SessionStorage.Serialization;
using Newtonsoft.Json;

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
            builder.Services.Replace(ServiceDescriptor.Scoped<IJsonSerializer, NewtonSoftJsonSerializer>());
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazorFluentUI();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddNotifications();
            builder.Services.AddAuthentication();
            builder.Services.AddFirebaseAuth();
            await builder.Build().RunAsync();
        }
    }

    public class NewtonSoftJsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string text)
            => JsonConvert.DeserializeObject<T>(text);

        public string Serialize<T>(T obj)
            => JsonConvert.SerializeObject(obj);
    }
}
