using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NetduinoGarageDoorOpener
{
    public class WebServer : IDisposable
    {
        private Socket socket = null;
        //open connection to onbaord led so we can blink it with every request
        private OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private OutputPort Garage2CarOpener = new OutputPort(Pins.GPIO_PIN_D13, false);

        public WebServer()
        {
            //Initialize Socket class
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Request and bind to an IP from DHCP server
            socket.Bind(new IPEndPoint(IPAddress.Any, 80));
            //Debug print our IP address
            Debug.Print(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            //Start listen for web requests
            socket.Listen(10);
            ListenForRequest();
        }

        public void ListenForRequest()
        {
            while (true)
            {
                using (Socket clientSocket = socket.Accept())
                {
                    //Get clients IP
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    EndPoint clientEndPoint = clientSocket.RemoteEndPoint;
                    //int byteCount = cSocket.Available;
                    int bytesReceived = clientSocket.Available;
                    if (bytesReceived > 0)
                    {
                        //Get request
                        byte[] buffer = new byte[bytesReceived];
                        int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                        string request = new string(Encoding.UTF8.GetChars(buffer));
                        string firstLine = request.Substring(0, request.IndexOf('\n')); //Example "GET /activatedoor HTTP/1.1"
                        string[] words = firstLine.Split(' ');  //Split line into words
                        string command = string.Empty;
                        if( words.Length > 2)
                        {
                            string method = words[0]; //First word should be GET
                            command = words[1].TrimStart('/'); //Second word is our command - remove the forward slash
                        }
                        switch (command.ToLower())
                        {
                            case "activatedoor":
                                ActivateGarageDoor();
                                //Compose a response
                                string response = "I just opened or closed the garage!";
                                string header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                                clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                                clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
                                break;
                            default:
                                //Did not recognize command
                                response = "Bad command";
                                header = "HTTP/1.0 200 OK\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
                                clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                                clientSocket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
                                break;
                        }
                    }
                }
            }
        }
        private void ActivateGarageDoor()
        {
            led.Write(true);                //Light on-board LED for visual cue
            Garage2CarOpener.Write(true);   //"Push" garage door button
            Thread.Sleep(1000);             //For 1 second
            led.Write(false);               //Turn off on-board LED
            Garage2CarOpener.Write(false);  //Turn off garage door button
        }
        #region IDisposable Members
        ~WebServer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (socket != null)
                socket.Close();
        }
        #endregion
    }
}


