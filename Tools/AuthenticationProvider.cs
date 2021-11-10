using Blazored.SessionStorage;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class AuthenticationProvider : AuthenticationStateProvider
    {
        [Inject]
        private HttpClient httpClient { get; }
        [Inject]
        private ISessionStorageService sessionStorage { get; }
        [Inject]
        private ILocalStorageService localStorage { get; }

        public AuthenticationProvider(HttpClient httpClient, ISessionStorageService sessionStorage, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.sessionStorage = sessionStorage;
            this.localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // セッションストレージからトークンを取得
            var savedTaken = await sessionStorage.GetItemAsync<string>("authToken");
            var userID = await sessionStorage.GetItemAsync<string>("userID");

            // トークンが存在しない場合
            if (string.IsNullOrWhiteSpace(savedTaken) || string.IsNullOrWhiteSpace(userID))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // HTTP認証用のトークンを設定
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedTaken);

            //認証情報
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userID) }, "apiauth")));
        }

        public async Task MarkUserAsAuthenticated(string userID, string authToken)
        {
            // セッションストレージに認証情報を保持
            await sessionStorage.SetItemAsync("userID", userID);
            await sessionStorage.SetItemAsync("authToken", authToken);
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            // セッションストレージから認証情報を削除
            await sessionStorage.RemoveItemAsync("userID");
            await sessionStorage.RemoveItemAsync("authToken");
            await sessionStorage.RemoveItemAsync("firebaseAuth");
            if (httpClient.DefaultRequestHeaders.Authorization != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            return services.AddScoped<AuthenticationStateProvider, AuthenticationProvider>();
        }
    }
}
