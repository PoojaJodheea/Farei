
    using Microsoft.AspNetCore.SignalR;

    public class NotificationSetting : Hub
    {
        // Called by controller when pushing notification
        public async Task SendNotification(string user, string title, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", title, message);
        }
    }

