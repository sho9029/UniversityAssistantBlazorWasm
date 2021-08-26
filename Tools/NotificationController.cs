using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using UniversityAssistantBlazorWasm.Models;
using UniversityAssistantBlazorWasm.Properties;
using WebPush;

namespace UniversityAssistantBlazorWasm.Tools
{
    public static class NotificationController
    {
        public static async Task SendNotificationAsync(PushSubscription subscription, NotificationPayload payload)
        {
            var publicKey = Confidential.Firebase.PublicKey;
            var privateKey = Confidential.Firebase.PrivateKey;

            var pushSubscription = new PushSubscription(subscription.Endpoint, subscription.P256DH, subscription.Auth);
            var vapidDetails = new VapidDetails($"mailto:{Confidential.Firebase.Email}", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var _payload = JsonSerializer.Serialize(new
                {
                    title = payload.Title,
                    message = payload.Message,
                    url = payload.Url
                });
                await webPushClient.SendNotificationAsync(pushSubscription, _payload, vapidDetails);
            }
            catch (WebPushException ex)
            {
                Console.Error.WriteLine($"Http status code: {ex.StatusCode}");
                Console.Error.WriteLine($"Error sending push notification: {ex.Message}");
            }
        }

    }
}
