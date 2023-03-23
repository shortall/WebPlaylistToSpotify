using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WebPlaylistToSpotify.Auth
{
    /// <summary>
    /// Based on https://github.com/IdentityModel/IdentityModel.OidcClient SystemBrowser
    /// </summary>
    public abstract class OAuthBrowser
    {
        protected string? UsedCallbackUri { get; private set; }

        protected abstract string AuthUrl(string callbackUrl);
        protected abstract Task<string> PostProcess(string initialResponse);

        public async Task<string> Authorise()
        {
            var port = GetRandomUnusedPort();

            using (var listener = new LoopbackHttpListener(port))
            {
                UsedCallbackUri = listener.Url;
                var authUrl = AuthUrl(listener.Url);
                OpenBrowser(authUrl);

                var initialResponse = await listener.WaitForCallbackAsync();

                return await PostProcess(initialResponse);
            }
        }

        private static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
