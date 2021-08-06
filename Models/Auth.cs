using System;
using System.Threading.Tasks;

namespace UniversityAssistantBlazorWasm.Models
{
    public interface IAuthService
    {
        Task<SignInResult> SignInAsync(SignInModel SignInModel);
        Task<SignInResult> SignUpAsync(SignInModel signInModel);
        Task SignOutAsync();
    }

    public class SignInModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignInResult
    {
        public bool IsSuccessful { get; set; }
        public Exception Error { get; set; }
        public string IDToken { get; set; }
    }
}
