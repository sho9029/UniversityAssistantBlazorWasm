using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        private ILocalStorageService localStorage { get; }

        public AuthenticationProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // ローカルストレージからトークンを取得
            var savedTaken = await localStorage.GetItemAsync<string>("authToken");
            var userID = await localStorage.GetItemAsync<string>("userID");

            // トークンが存在しない場合
            if (string.IsNullOrWhiteSpace(savedTaken))
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
            // ローカルストレージに認証情報を保持
            await localStorage.SetItemAsync("userID", userID);
            await localStorage.SetItemAsync("authToken", authToken);
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            // ローカルストレージから認証情報を削除
            await localStorage.RemoveItemAsync("userID");
            await localStorage.RemoveItemAsync("authToken");
            if (httpClient.DefaultRequestHeaders.Authorization != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
