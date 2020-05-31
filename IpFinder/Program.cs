using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IpFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> ipReachable = new List<string>();

            string[] ips = GetIps();

            Console.WriteLine("Scanning local network...");

            Parallel.ForEach(ips, async (ip) =>
            {
                using (Ping ping = new Ping())
                {
                    var response = await ping.SendPingAsync(ip);

                    //Console.WriteLine($"{ip} [{response.Status}] ({Thread.CurrentThread.ManagedThreadId})");

                    if (response.Status == IPStatus.Success)
                    {
                        ipReachable.Add(ip);
                    } 
                }
            });

            ipReachable.Sort();

            Console.Clear();
            Console.WriteLine($"IPs found ({ipReachable.Count}):");

            foreach (var ip in ipReachable)
            {
                string result = string.Empty;
                try
                {
                    result = Dns.GetHostEntry(ip).HostName;
                }
                catch (SocketException)
                {
                }

                Console.WriteLine($"{ip} {result}");
            }
        }

        private static string[] GetIps()
        {
            List<string> ips = new List<string>();

            for (int j = 0; j < 256; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    ips.Add($"192.168.{j}.{i}");
                }
            }

            return ips.ToArray();
        }
    }
}
