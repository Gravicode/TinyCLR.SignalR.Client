# TinyCLR.SignalR.Client
This is experimental project to test SignalR on TinyCLR

## On SignalR Host
```
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
```
## On TinyCLR Device
```
using TinyCLR.SignalR.Client;

static void StartSignalR()
        {
            //setup connection
            var options = new HubConnectionOptions() { Reconnect = true };
            //change to your ip signalr host
            HubConnection hubConnection = new HubConnection($"http://{HostIP}/deviceHub", options: options);

            hubConnection.Closed += HubConnection_Closed;

            hubConnection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
            {
                var deviceid = args[0];
                var message = args[1];

                Debug.WriteLine($"{deviceid} : {message}");
            });

            //start connection
            hubConnection.Start();

            int count = 0;
            while (hubConnection.State == HubConnectionState.Connected)
            {
                hubConnection.InvokeCore("SendMessage", null, new object[] { "tinyclr device", $"sending message - {count}" });
                count++;
                Thread.Sleep(1000);
            }


            //hubConnection.Stop("client failed to connect");

        }
        private static void HubConnection_Closed(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine($"closed received with message: {message.Message}");
        }
```
Note: Ported from [NanoFramework SignalR Client](https://github.com/nanoframework/nanoFramework.SignalR.Client)

Enjoy and Cheers :D.

-Buitenzorg Makers Club
