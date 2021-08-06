using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class AuthenticationProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // ローカルストレージからトークンを取得
            var savedTaken = await _localStorage.GetItemAsync<string>("authToken");
            var userID = await _localStorage.GetItemAsync<string>("userID");

            // トークンが存在しない場合
            if (string.IsNullOrWhiteSpace(savedTaken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // HTTP認証用のトークンを設定
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedTaken);

            //認証情報
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userID) }, "apiauth")));
        }

        public async Task MarkUserAsAuthenticated(string userID, string authToken)
        {
            // ローカルストレージに認証情報を保持
            await _localStorage.SetItemAsync("userID", userID);
            await _localStorage.SetItemAsync("authToken", authToken);
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            // ローカルストレージから認証情報を削除
            await _localStorage.RemoveItemAsync("userID");
            await _localStorage.RemoveItemAsync("authToken");
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            // 認証情報の変更通知
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
