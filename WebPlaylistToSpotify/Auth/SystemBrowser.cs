using SpotifyAPI.Web;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WebPlaylistToSpotify.Auth
{
    public class SystemBrowser
    {
        private const string Sha256CodeChallengeMethod = "S256";

        private readonly string _clientId;

        private string _verifier;
        private string _usedCallbackUri;

        public SystemBrowser(string clientId)
        {
            _clientId = clientId;
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private string AuthUrl(string callbackUrl)
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            _verifier = verifier;

            var loginRequest = new LoginRequest(
              new Uri(callbackUrl),
              _clientId,
              LoginRequest.ResponseType.Code
            )
            {
                CodeChallengeMethod = Sha256CodeChallengeMethod,
                CodeChallenge = challenge,
                Scope = new[] { Scopes.PlaylistModifyPublic }
            };
            return loginRequest.ToUri().ToString();
        }

        public async Task<string> GetToken(string code)
        {
            var initialResponse = await new OAuthClient().RequestToken(
                new PKCETokenRequest(_clientId, code, new Uri(_usedCallbackUri), _verifier)
            );

            return initialResponse.AccessToken;
        }

        public async Task<string> InvokeAsync()
        {
            var port = GetRandomUnusedPort();

            using (var listener = new LoopbackHttpListener(port))
            {
                _usedCallbackUri = listener.Url;
                var authUrl = AuthUrl(listener.Url);
                OpenBrowser(authUrl);

                return await listener.WaitForCallbackAsync();
            }
        }

        private void OpenBrowser(string url)
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
    }
}
