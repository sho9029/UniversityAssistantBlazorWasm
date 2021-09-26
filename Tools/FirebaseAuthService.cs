using Blazored.SessionStorage;
using Firebase.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using UniversityAssistantBlazorWasm.Models;
using UniversityAssistantBlazorWasm.Properties;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class FirebaseAuthService : IAuthService
    {
        [Inject]
        private ISessionStorageService sessionStorage { get; set; }
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; }

        private FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));

        public FirebaseAuthService(ISessionStorageService sessionStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            this.sessionStorage = sessionStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        private async Task<FirebaseAuthLink> GetFirebaseAuthLinkAsync()
        {
            return await sessionStorage.GetItemAsync<FirebaseAuthLink>("firebaseAuth");
        }

        private async Task SetFirebaseAuthLinkAsync(FirebaseAuthLink firebaseAuthLink)
        {
            await sessionStorage.SetItemAsync("firebaseAuth", firebaseAuthLink);
        }

        public async Task<SignInResult> SignInAsync(SignInModel signInModel)
        {
            try
            {
                await SetFirebaseAuthLinkAsync(await provider.SignInWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password));
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = (await (await GetFirebaseAuthLinkAsync()).GetFreshAuthAsync()).FirebaseToken
                };
                await ((AuthenticationProvider)authenticationStateProvider).MarkUserAsAuthenticated(signInModel.Email, res.IDToken);
                return res;
            }
            catch (FirebaseAuthException ex)
            {
                return new SignInResult()
                {
                    IsSuccessful = false,
                    Error = ex
                };
            }
        }

        public async Task<SignInResult> SignUpAsync(SignInModel signInModel)
        {
            try
            {
                await SetFirebaseAuthLinkAsync(await provider.CreateUserWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password));
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = (await (await GetFirebaseAuthLinkAsync()).GetFreshAuthAsync()).FirebaseToken
                };
                await ((AuthenticationProvider)authenticationStateProvider).MarkUserAsAuthenticated(signInModel.Email, res.IDToken);
                return res;
            }
            catch (FirebaseAuthException ex)
            {
                return new SignInResult()
                {
                    IsSuccessful = false,
                    Error = ex
                };
            }
        }

        public async Task SignOutAsync()
        {
            await ((AuthenticationProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<string> GetFreshTokenAsync()
        {
            return (await (await GetFirebaseAuthLinkAsync()).GetFreshAuthAsync()).FirebaseToken;
        }

        public async Task<string> GetUidAsync()
        {
            var user = await provider.GetUserAsync(await GetFreshTokenAsync());
            return user.LocalId;
        }

        public async Task<string> GetDisplayNameAsync()
        {
            var user = await provider.GetUserAsync(await GetFreshTokenAsync());
            return user.DisplayName;
        }
    }

    public static class AuthExtension
    {
        public static IServiceCollection AddFirebaseAuth(this IServiceCollection services)
        {
            return services.AddScoped<IAuthService, FirebaseAuthService>();
        }
    }
}
