
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace HttpClientLoop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunLoop(int.Parse(args.FirstOrDefault() ?? "8123"))
                .Wait();
        }

        private static async Task RunLoop(int port)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var handler = new SocketsHttpHandler()
            {
                
            };


            var client = new HttpClient(handler);

            var i = 0;

            while(true)
            {

                try
                {
                    var url = $"https://localhost:{port}/{i++}";
                    var r = await client.GetStringAsync(url);
                    Console.WriteLine($"{url} -> {r}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                await Task.Delay(10000);
            }

        }
    }
}
