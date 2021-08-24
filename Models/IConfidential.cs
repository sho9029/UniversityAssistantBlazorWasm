using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace UniversityAssistantBlazorWasm.Models
{
    public interface IConfidential
    {
        public static class Firebase
        {
            public static readonly string ApiKey;
            public static readonly string DatabaseUrl;
            public static readonly string PublicKey;
            public static readonly string PrivateKey;
        }

        [JSInvokable]
        public string GetApiKey()
        {
            return Firebase.ApiKey;
        }

        [JSInvokable]
        public string GetPublicKey()
        {
            return Firebase.PublicKey;
        }
    }
}
