using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace ipio
{

    //Utility Is Here To Abstract Some Code In Program.cs To Make Is Cleaner And Easier To Read
    class utility
    {
        public static bool pingCom(string hostName) { 
            Ping ping = new();
            PingOptions options = new();
            PingReply reply = ping.Send(hostName, 10,  Encoding.ASCII.GetBytes("69696969696969696969696969696969"), options);

            return reply.Status == IPStatus.Success;
        }

        //Unused Function Because Is It Really, Really Slow
        public static List<String> pingNetwork()
        {
            List<String> ipAddresses = new();

            for (int i = 0; i < 255; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (pingCom($"192.168.{i}.{j}")) { ipAddresses.Add($"192.168.{i}.{j}");  }
                    Console.WriteLine($"192.168.{i}.{j}");
                }
            }
            return ipAddresses;
        }

        public static string sendRequest(IPAddress ip, string protocol, int port, string message, byte[] messageBackBytes) {
            SocketType socketType = SocketType.Stream;
            ProtocolType protocolType = ProtocolType.Tcp;
            if (protocol.ToLower() == "udp") { Console.WriteLine("test"); socketType = SocketType.Dgram; protocolType = ProtocolType.Udp; }
            IPEndPoint endPoint = new(ip, port);
            Socket socket = new(ip.AddressFamily, socketType, protocolType);
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.Connect(endPoint);
            socket.Send(data);
            int receivedData = socket.Receive(messageBackBytes);
            return Encoding.ASCII.GetString(messageBackBytes, 0, receivedData);

        }

        public async static Task<string> httpRequest(Uri url, HttpMethod method, string requestUri, Version version, Dictionary<string, string> headers)
        {
            
            HttpClient client = new() { BaseAddress=url};
            //Uses Firefox/Windows User Agent String
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:142.0) Gecko/20100101 Firefox/142.0");
            foreach (var header in headers) { client.DefaultRequestHeaders.Add(header.Key, header.Value); }
            HttpRequestMessage request = new(method, requestUri);
            request.Version = version;
            HttpResponseMessage response = await client.SendAsync(request);
            return response.StatusCode.ToString();
        }
    }
}
