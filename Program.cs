using ipio;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;

try
{
    //Main Program, For Cleaner Code Uses Functions In utility.cs
    Console.WriteLine("IPIO Alpha 0.1.0");
    if (args.Length == 0) { Console.Write(">>"); args = Console.ReadLine().Split(" "); }

    if (args[0].ToLower() == "utility")
    {
        switch (args[1].ToLower())
        {
            case ("help"):
                {
                    if (args.Length < 3) { Console.WriteLine("Provides A Set Of Utility Functions\nCommands:\n\tGetHostEntry\n\tPing\n\tComHostName"); break; }
                    switch (args[2].ToLower())
                    {
                        case ("gethostentry"): Console.WriteLine("GetHostEntry: Get's A Host Name And Ip Address From A Hostname Or IP Address\nTakes One Argument: A Host Name Or IP Address"); break;
                        case ("ping"): Console.WriteLine("Ping: Pings A Host To See If It Is Reachable\nTakes One Argument: A Host Name Or IP Address"); break;
                        case ("comhostname"): Console.WriteLine("ComHostName: Gets The Computer's Host Name\nTakes No Arguments"); break;
                        default: Console.WriteLine($"{args[2].ToLower()} Is Not A Utility Function"); break;
                    }
                    break;

                }
            case ("gethostentry"):
                {
                    if (args.Length < 3) { Console.WriteLine("ERROR: No Hostname Or IP Address Provided"); break; }
                    var host = Dns.GetHostEntry(args[2]);
                    Console.WriteLine($"Host Name: {host.HostName}");
                    foreach (var ip in host.AddressList)
                        Console.WriteLine($"IP Address: {ip}");
                    break;
                }
            case ("ping"):
                {
                    if (args.Length < 3) { Console.WriteLine("ERROR: No Hostname Or IP Address Provided"); break; }
                    Console.WriteLine(utility.pingCom(args[2]) ? "Succeded" : "Could Not Reach Host"); break;
                }
            case ("comhostname"):
                {
                    Console.WriteLine(Dns.GetHostName()); break;
                }
            default: Console.WriteLine("Unknown Command"); break;

        }
    }
    else if (args[0].ToLower() == "tcp" || args[0].ToLower() == "udp")
    {
        args[^1] += ' ';
        string[] requestArgs = string.Join(" ", args[1..]).Split('-')[1..];
        string ipAddress = "";
        string port = "";
        string message = "";
        string receiveBytes = "";

        foreach (var arg in requestArgs)
        {
            string[] splitArg = arg.Split(" ")[1..];
            string afterArgName = string.Join(" ", splitArg[0..(splitArg.Length - 1)]);
            string argName = arg.Split(" ")[0];

            switch (argName.ToLower())
            {
                case ("ip" or "i"): ipAddress = afterArgName; break;
                case ("port" or "p"): port = afterArgName; break;
                case ("message" or "m"): message = afterArgName; break;
                case ("receivebytes" or "r"): receiveBytes = afterArgName; break;
                default: Console.WriteLine($"Unknown Argument: {argName}"); break;
            }

        }

        Console.WriteLine(utility.sendRequest(IPAddress.Parse(ipAddress), args[0], Convert.ToInt32(port), message, new byte[Convert.ToInt32(receiveBytes)]));
    }

    else if (args[0].ToLower().Split("/")[0] == "http")
    {
        args[^1] += ' ';

        //Gets HTTP Version (Defaults To 1.1 If Not Provided)
        string version = "1";
        if (args[0].ToLower() != "http") { version = args[0].ToLower().Split("/")[1].Split(".")[0]; }
        if (version == "1") { version += ".1"; }
        else { version += ".0"; }

        string[] requestArgs = string.Join(" ", args[1..]).Split('-')[1..];
        string url = "";
        string method = "GET";
        string requestUri = "/";
        Dictionary<string, string> headers = new();

        foreach (var arg in requestArgs)
        {
            string[] splitArg = arg.Split(" ")[1..];
            string afterArgName = string.Join(" ", splitArg[0..(splitArg.Length - 1)]);
            string argName = arg.Split(" ")[0];

            switch (argName.ToLower())
            {
                case ("url" or "u"): url = afterArgName; break;
                case ("method" or "m"): method = afterArgName.ToUpper(); break;
                case ("requesturi" or "r"): requestUri = afterArgName; break;
                case ("header" or "h"):
                    string[] headerSplit = afterArgName.Split(":");
                    headers.Add(headerSplit[0], string.Join(":", headerSplit[1..]));
                    break;
            }

        }

        if (url[0..4] != "http") { url = "http://" + url; }

        Console.WriteLine(await utility.httpRequest(new Uri(url), new HttpMethod(method), requestUri, new Version(version), headers));
    }

    else if (args[0].ToLower() == "help") { Console.WriteLine("Commands:\n\tUtility: Utility Functions\n\tTCP: Sends A Request Using TCP\n\tUDP Sends A Request Using UDP\n\tHTTP: Sends A Request Using The Specified Http Version"); }
    else { Console.WriteLine("Unknown Command"); }

}
catch (Exception e) { Console.WriteLine($"ERROR: {e.Message}"); }