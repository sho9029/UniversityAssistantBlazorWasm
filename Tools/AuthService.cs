using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Timers;
using UniversityAssistantBlazorWasm.Models;

namespace UniversityAssistantBlazorWasm.Tools
{
    public class AuthService : IAuthService
    {
        [Inject]
        private ISessionStorageService sessionStorage { get; set; }
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; }

        public AuthService(ISessionStorageService sessionStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            this.sessionStorage = sessionStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }


        public async Task<SignInResult> SignInAsync(SignInModel signInModel)
        {
            return new SignInResult { IsSuccessful = false, IDToken = null };
        }

        public async Task<SignInResult> SignUpAsync(SignInModel signInModel)
        {
            return new SignInResult { IsSuccessful = false, IDToken = null };
        }

        public async Task SignOutAsync()
        {
            await ((AuthenticationProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        }


        public async Task<string> GetUidAsync()
        {
            return "";
        }

        public async Task<string> GetDisplayNameAsync()
        {
            return "";
        }

    }

    public static class AuthExtension
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            return services.AddScoped<IAuthService, AuthService>();
        }
    }
}
