using Blazored.SessionStorage;
using Firebase.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Timers;
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
        private static Timer timer;

        public FirebaseAuthService(ISessionStorageService sessionStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            this.sessionStorage = sessionStorage;
            this.authenticationStateProvider = authenticationStateProvider;
            SetTimer();
        }

        private async Task<FirebaseAuthLink> GetFirebaseAuthLinkAsync()
        {
            if (!await sessionStorage.ContainKeyAsync("firebaseAuth"))
            {
                await SignOutAsync();
                throw new NullReferenceException("firebaseAuth is not contained in SessionStorage.");
            }

            return await sessionStorage.GetItemAsync<FirebaseAuthLink>("firebaseAuth");
        }

        private async Task SetFirebaseAuthLinkAsync(FirebaseAuthLink firebaseAuthLink)
        {
            await sessionStorage.SetItemAsync("firebaseAuth", firebaseAuthLink);
        }

        private void SetTimer()
        {
            timer = new Timer(600000);
            timer.Elapsed += async (s, e) => await GetFreshAuthAsync(s, e);
            timer.Start();
        }

        private void DisposeTimer()
        {
            if (timer is not null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        private async Task GetFreshAuthAsync(object s, ElapsedEventArgs e)
        {
            if (!(await authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                return;
            }

            await SetFirebaseAuthLinkAsync(await (await GetFirebaseAuthLinkAsync()).GetFreshAuthAsync());
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
                SetTimer();
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
                var token = (await (await GetFirebaseAuthLinkAsync()).GetFreshAuthAsync()).FirebaseToken;
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = token
                };
                await ((AuthenticationProvider)authenticationStateProvider).MarkUserAsAuthenticated(signInModel.Email, res.IDToken);
                await provider.UpdateProfileAsync(token, signInModel.DisplayName, null);
                SetTimer();
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
            DisposeTimer();
        }

        public async Task<string> GetFirebaseTokenAsync()
        {
            return (await GetFirebaseAuthLinkAsync()).FirebaseToken;
        }

        public async Task<string> GetUidAsync()
        {
            var user = await provider.GetUserAsync(await GetFirebaseTokenAsync());
            return user.LocalId;
        }

        public async Task<string> GetDisplayNameAsync()
        {
            var user = await provider.GetUserAsync(await GetFirebaseTokenAsync());
            return user.DisplayName;
        }

        public async Task UpdateProfileAsync(string displayName, string photoUrl)
		{
            await provider.UpdateProfileAsync(await GetFirebaseTokenAsync(), displayName, photoUrl);
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
