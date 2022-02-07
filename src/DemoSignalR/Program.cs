using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using TinyCLR.SignalR.Client;

namespace DemoSignalR
{
    internal class Program
    {
        const string HostIP = "192.168.0.144";
        static void Main()
        {
            EthernetTest();
            if(TestHttp())
                StartSignalR();
            Thread.Sleep(Timeout.Infinite);
        }

        static bool TestHttp()
        {
            var url = $"http://{HostIP}/index.html";

            int read = 0, total = 0;
            byte[] result = new byte[512];

            try
            {
                using (var req = HttpWebRequest.Create(url) as HttpWebRequest)
                {
                    req.KeepAlive = false;
                    req.ReadWriteTimeout = 5000;

                    using (var res = req.GetResponse() as HttpWebResponse)
                    {
                        using (var stream = res.GetResponseStream())
                        {
                            do
                            {
                                read = stream.Read(result, 0, result.Length);
                                total += read;

                                Debug.WriteLine("read : " + read);
                                Debug.WriteLine("total : " + total);

                                String page = "";

                                page = new String(System.Text.Encoding.UTF8.GetChars
                                    (result, 0, read));

                                Debug.WriteLine("Response : " + page);
                               
                            }

                            while (read != 0);
                            return true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
               Debug.WriteLine(ex.ToString());
                return false;
            }
        }
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

        static bool linkReady = false;

        static void EthernetTest()
        {
            //Reset external phy.
            var gpioController = GpioController.GetDefault();
            var resetPin = gpioController.OpenPin(SC20260.GpioPin.PG3);

            resetPin.SetDriveMode(GpioPinDriveMode.Output);

            resetPin.Write(GpioPinValue.Low);
            Thread.Sleep(100);

            resetPin.Write(GpioPinValue.High);
            Thread.Sleep(100);

            var networkController = NetworkController.FromName(SC20260.NetworkController.EthernetEmac);

            var networkInterfaceSetting = new EthernetNetworkInterfaceSettings();

            var networkCommunicationInterfaceSettings = new BuiltInNetworkCommunicationInterfaceSettings();

            networkInterfaceSetting.Address = new IPAddress(new byte[] { 192, 168, 1, 122 });
            networkInterfaceSetting.SubnetMask = new IPAddress(new byte[] { 255, 255, 255, 0 });
            networkInterfaceSetting.GatewayAddress = new IPAddress(new byte[] { 192, 168, 1, 1 });

            networkInterfaceSetting.DnsAddresses = new IPAddress[]
                {new IPAddress(new byte[] { 75, 75, 75, 75 }),
         new IPAddress(new byte[] { 75, 75, 75, 76 })};

            networkInterfaceSetting.MacAddress = new byte[] { 0x00, 0x4, 0x00, 0x00, 0x00, 0x00 };
            networkInterfaceSetting.DhcpEnable = true;
            networkInterfaceSetting.DynamicDnsEnable = true;

            networkController.SetInterfaceSettings(networkInterfaceSetting);
            networkController.SetCommunicationInterfaceSettings(networkCommunicationInterfaceSettings);

            networkController.SetAsDefaultController();

            networkController.NetworkAddressChanged += NetworkController_NetworkAddressChanged;
            networkController.NetworkLinkConnectedChanged += NetworkController_NetworkLinkConnectedChanged;

            networkController.Enable();

            while (linkReady == false) ;
            Debug.WriteLine("Network is ready to use");
            //Thread.Sleep(Timeout.Infinite);
        }

        private static void NetworkController_NetworkLinkConnectedChanged
            (NetworkController sender, NetworkLinkConnectedChangedEventArgs e)
        {
            //Raise connect/disconnect event.
        }

        private static void NetworkController_NetworkAddressChanged
            (NetworkController sender, NetworkAddressChangedEventArgs e)
        {

            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();

            linkReady = address[0] != 0;
            Debug.WriteLine("IP: " + address[0] + "." + address[1] + "." + address[2] + "." + address[3]);
        }
    }
}
