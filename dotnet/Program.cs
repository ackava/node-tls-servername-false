
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HttpClientLoop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunLoop(int.TryParse(args.LastOrDefault(), out var port) ? port : 8123)
                .Wait();
        }

        private static async Task RunLoop(int port)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var sslOptions = new SslClientAuthenticationOptions
            {
                // Leave certs unvalidated for debugging
                RemoteCertificateValidationCallback = delegate { return true; },
                // AllowTlsResume = true,
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                // AllowRenegotiation = true
            };
            var handler = new SocketsHttpHandler()
            {
                SslOptions = sslOptions,
                // KeepAlivePingDelay = TimeSpan.FromSeconds(1),
                // KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
                // KeepAlivePingTimeout = TimeSpan.FromSeconds(1)
            };

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var s = 0;

            var list = new List<Task>();

            for(int i=0;i<100;i++) {
                var url = $"https://localhost:{port}/{s++}";
                var client = new HttpClient(handler);
                var msg = new HttpRequestMessage(HttpMethod.Get, url);
                msg.Headers.TryAddWithoutValidation("keep-alive", "close");
                Console.WriteLine($"{url} -> ");
                using var r = await client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead);
                Console.WriteLine(await r.Content.ReadAsStringAsync());

                await Task.Delay(5000);
                System.GC.Collect();
                await Task.Delay(5000);
                System.GC.Collect();

            }

            await Task.WhenAll(list);

            // while(true)
            // {

            //     try
            //     {


            //         var url = $"https://localhost:{port}/{s++}";
            //         var msg = new HttpRequestMessage(HttpMethod.Get, url);
            //         // msg.Headers.TryAddWithoutValidation("keep-alive", "close");
            //         Console.Write($"{url} -> ");
            //         using var r = await client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead);
            //         Console.WriteLine(await r.Content.ReadAsStringAsync());
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex.ToString());
            //     }
            //     await Task.Delay(100);
            // }

        }
    }
}
