using SpotifyAPI.Web;

namespace WebPlaylistToSpotify.Auth
{
    public class SpotifyOAuthBrowser : OAuthBrowser
    {
        private const string Sha256CodeChallengeMethod = "S256";

        private readonly string _clientId;
        
        private string? _verifier;

        public SpotifyOAuthBrowser(string clientId)
        {
            _clientId = clientId;
        }

        protected override string AuthUrl(string callbackUrl)
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
                new PKCETokenRequest(_clientId, code, new Uri(usedCallbackUri!), _verifier!)
            );

            return initialResponse.AccessToken;
        }
    }
}
