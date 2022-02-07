using Microsoft.AspNetCore.SignalR;

namespace WebSignalR.Hubs
{
    public class DeviceHub : Hub
    {
        public async Task SendMessage(string deviceid, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", deviceid, message);
        }
    }
}