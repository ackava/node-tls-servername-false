
using System.Net;
using System.Net.Security;
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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var sslOptions = new SslClientAuthenticationOptions
            {
                // Leave certs unvalidated for debugging
                RemoteCertificateValidationCallback = delegate { return true; }
            };

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var handler = new SocketsHttpHandler()
            {
                SslOptions = sslOptions
            };

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.TryAddWithoutValidation("keep-alive", "close");
            client.DefaultRequestVersion = new Version(2, 0);
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;

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

                await Task.Delay(1000);
            }

        }
    }
}
