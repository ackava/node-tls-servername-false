
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
                RemoteCertificateValidationCallback = delegate { return true; },
                AllowTlsResume = true,
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls13,
                AllowRenegotiation = true
            };
            var handler = new SocketsHttpHandler()
            {
                SslOptions = sslOptions,
                KeepAlivePingDelay = TimeSpan.FromSeconds(1),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests,
                KeepAlivePingTimeout = TimeSpan.FromSeconds(1)
            };

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            var s = 0;

            while(true)
            {

                try
                {


                    using var client = new HttpClient(handler, false);
                    var url = $"https://localhost:{port}/{s++}";
                    var msg = new HttpRequestMessage(HttpMethod.Get, url);
                    msg.Headers.TryAddWithoutValidation("keep-alive", "close");
                    using var r = await client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead);
                    Console.WriteLine($"{url} -> {await r.Content.ReadAsStringAsync()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("Running GC for 2 minutes");
                for (int j = 0; j < 61; j++)
                {
                    Console.Write(".");
                    var b = new byte[1024 * 1024 * 512];
                    await Task.Delay(1000);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    GC.WaitForPendingFinalizers();
                }
                Console.WriteLine("");
            }

        }
    }
}
