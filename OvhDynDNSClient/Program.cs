using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OvhDynDNSClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Please pass all required arguments");
                return;
            }
            var domain = args[0];
            var login = args[1];
            var password = args[2];
            
            using var client = new HttpClient();

            var localIp = await client.GetAsync("http://icanhazip.com").Result.Content.ReadAsStringAsync();
            localIp = localIp.Trim();
            var serverIp = (await Dns.GetHostAddressesAsync(domain))[0].ToString();

            if (localIp == serverIp)
                return;
            var byteArray = Encoding.ASCII.GetBytes($"{login}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            string endpoint = $"http://www.ovh.com/nic/update?system=dyndns&hostname={domain}&myip={localIp}";
            var result = await client.GetAsync(endpoint);
            Console.WriteLine(result.StatusCode);
            System.Threading.Thread.Sleep(5000);
        }
    }
}