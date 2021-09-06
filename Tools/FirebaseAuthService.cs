using Firebase.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using UniversityAssistantBlazorWasm.Models;
using UniversityAssistantBlazorWasm.Properties;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class FirebaseAuthService : IAuthService
    {
        private FirebaseAuthLink FirebaseAuthLink { get; set; }
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; }
        public FirebaseAuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<SignInResult> SignInAsync(SignInModel signInModel)
        {
            try
            {
                var provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));
                FirebaseAuthLink = await provider.SignInWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password);
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = (await FirebaseAuthLink.GetFreshAuthAsync()).FirebaseToken
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

        public async Task<SignInResult> SignInWithTokenAsync(string token)
        {
            try
            {
                var provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));
                FirebaseAuthLink = await provider.SignInWithCustomTokenAsync(token);
                var res = new SignInResult
                {
                    IsSuccessful = true,
                    IDToken = (await FirebaseAuthLink.GetFreshAuthAsync()).FirebaseToken
                };
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
                var provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));
                FirebaseAuthLink = await provider.CreateUserWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password);
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = (await FirebaseAuthLink.GetFreshAuthAsync()).FirebaseToken
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

        public async Task<string> GetFreshTokenAsync(string token)
        {
            var provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));
            FirebaseAuthLink = await provider.SignInWithCustomTokenAsync(token);
            return (await FirebaseAuthLink.GetFreshAuthAsync()).FirebaseToken;
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
