using System;
using System.Threading.Tasks;

namespace UniversityAssistantBlazorWasm.Models
{
    public interface IAuthService
    {
        Task<SignInResult> SignInAsync(SignInModel signInModel);
        Task<SignInResult> SignInWithTokenAsync(string token);
        Task<SignInResult> SignUpAsync(SignInModel signInModel);
        Task SignOutAsync();
        Task<string> GetFreshTokenAsync(string token);
        Task<string> GetUidAsync(string token);
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
        public string ErrorReason
        {
            get
            {
                if (Error == null)
                {
                    return String.Empty;
                }
                var message = Error.Message;
                if (message[(message.IndexOf("Response: ") + 10)..].StartsWith("N/A"))
                {
                    return "Unable to connect to Firebase authentication server.";
                }
                return message[(message.IndexOf("Reason: ") + 7)..];
            }
        }
        public string IDToken { get; set; }
    }
}
