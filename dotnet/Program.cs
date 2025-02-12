
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
            var sslOptions = new SslClientAuthenticationOptions
            {
                // Leave certs unvalidated for debugging
                RemoteCertificateValidationCallback = delegate { return true; },
                AllowTlsResume = true,
                AllowRenegotiation = true,
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                TargetHost = "localhost",
            };

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var handler = new SocketsHttpHandler()
            {
                SslOptions = sslOptions                
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

                await Task.Delay(60000);
            }

        }
    }
}
