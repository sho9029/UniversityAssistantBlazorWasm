namespace UniversityAssistantBlazorWasm.Models
{
    public class NotificationPayload
    {
        public NotificationPayload(string message, string url, string title = "UniversityAssistant")
        {
            Title = title;
            Message = message;
            Url = url;
        }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}
