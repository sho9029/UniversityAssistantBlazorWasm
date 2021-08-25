using Firebase.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using UniversityAssistantBlazorWasm.Models;
using UniversityAssistantBlazorWasm.Properties;


/*
 * Confidential.cs in "Properties" namespace
 * 
 * public static class Confidential
 * {
 *     public static readonly Dictionary<string, string> Firebase = new()
 *     {
 *         { "ApiKey", "Firebase api key" },
 *         { "DatabaseURL", "Database URL" },
 *         { "PublicKey", "Public key" }
 *     };
 * }
 */

namespace UniversityAssistantBlazorWasm.Tools
{
    public class FirebaseAuthService : IAuthService
    {
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
                var firebaseResult = await provider.SignInWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password);
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = firebaseResult.FirebaseToken
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
                var provider = new FirebaseAuthProvider(new FirebaseConfig(Confidential.Firebase.ApiKey));
                var firebaseResult = await provider.CreateUserWithEmailAndPasswordAsync(signInModel.Email, signInModel.Password);
                var res = new SignInResult()
                {
                    IsSuccessful = true,
                    IDToken = firebaseResult.FirebaseToken
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
    }
}
