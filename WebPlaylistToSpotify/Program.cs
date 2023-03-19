using HtmlAgilityPack;
using IdentityModel.OidcClient;
using SpotifyAPI.Web;
using System;
using System.Diagnostics;
using System.Web;
using WebPlaylistToSpotify;
using WebPlaylistToSpotify.Extensions;
using WebPlaylistToSpotify.Model;


try
{
    var appConfig = new AppConfig();


    // Generates a secure random verifier of length 100 and its challenge
    var (verifier, challenge) = PKCEUtil.GenerateCodes();

    var loginRequest = new SpotifyAPI.Web.LoginRequest(
      new Uri("http://localhost:5000/callback"),
      "cecbc33419334a7e968eddbd26639cc0",
      SpotifyAPI.Web.LoginRequest.ResponseType.Code
    )
    {
        CodeChallengeMethod = "S256",
        CodeChallenge = challenge,
        Scope = new[] { Scopes.PlaylistModifyPublic }
    };
    var uri = loginRequest.ToUri();

    var browser = new SystemBrowser();

    //var ps = new ProcessStartInfo();

    //ps.FileName = uri.ToString();
    //ps.CreateNoWindow = true;
    //ps.UseShellExecute = true;

    //Process.Start(ps);

    //Console.WriteLine("+-----------------------+");
    //Console.WriteLine("|  Sign in with OIDC    |");
    //Console.WriteLine("+-----------------------+");
    //Console.WriteLine("");
    //Console.WriteLine("Press any key to sign in...");
    //Console.ReadKey();

    //var browser = new SystemBrowser(45656);
    //string redirectUri = "http://127.0.0.1:45656";

    //var options = new OidcClientOptions
    //{
    //    Authority = _authority,
    //    ClientId = "native.code",
    //    RedirectUri = redirectUri,
    //    Scope = "openid profile native_api",
    //    FilterClaims = false,
    //    Browser = browser,
    //    LoadProfile = true
    //};

    //var _oidcClient = new OidcClient(options);
    //var result = await _oidcClient.LoginAsync(new IdentityModel.OidcClient.LoginRequest());
    //ShowResult(result);



    //var initialResponse = await GetCallback("cecbc33419334a7e968eddbd26639cc0", verifier);

    //var spotify = new SpotifyClient(initialResponse.AccessToken);


    /////////////////////////////////////////////////////
    ///Client Creds
    //var config = SpotifyClientConfig.CreateDefault();
    //var request = new ClientCredentialsRequest("cecbc33419334a7e968eddbd26639cc0", "595a60da144644a5bddaf5783b190534");
    //var response = await new OAuthClient(config).RequestToken(request);
    //var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
    /////////////////////////////////////////////////////

    Console.WriteLine("Starting...");

    var playlist = await CreatePlaylist(appConfig, spotify);

    await AddTracks(appConfig, spotify, playlist);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Console.WriteLine($"Hit <Enter> to finish");
    Console.ReadLine();
}

static void ShowResult(LoginResult result)
{
    if (result.IsError)
    {
        Console.WriteLine("\n\nError:\n{0}", result.Error);
        return;
    }

    Console.WriteLine("\n\nClaims:");
    foreach (var claim in result.User.Claims)
    {
        Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
    }

    Console.WriteLine($"\nidentity token: {result.IdentityToken}");
    Console.WriteLine($"access token:   {result.AccessToken}");
    Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
}

static async Task<PKCETokenResponse> GetCallback(string code, string verifier)
{
    // Note that we use the verifier calculated above!
    return await new OAuthClient().RequestToken(
      new PKCETokenRequest("ClientId", code, new Uri("http://localhost:5000"), verifier)
    );
}

static async Task AddTracks(AppConfig appConfig, SpotifyClient spotify, FullPlaylist spotifyPlaylist)
{
    using (var httpClient = new HttpClient())
    {
        foreach (var webPlaylist in appConfig.WebPlaylists)
        {
            Console.WriteLine($"Downloading playlist: {webPlaylist.Url}");
            var html = await httpClient.GetStringAsync(webPlaylist.Url);

            await AddWebPlaylist(spotify, spotifyPlaylist, html, webPlaylist.TrackNamesXPath);
        }

        Console.WriteLine("Done");
    }
}

static async Task AddWebPlaylist(SpotifyClient spotify, FullPlaylist spotifyPlaylist, string webPlaylistHtml, string? trackNamesXPath)
{
    if (spotifyPlaylist?.Id == null)
    {
        throw new ArgumentNullException("playlist.Id");
    }

    var doc = new HtmlDocument();
    doc.LoadHtml(webPlaylistHtml);

    var tracks = doc.DocumentNode
        .SelectNodes(trackNamesXPath)
        .Select(x => HttpUtility.HtmlDecode(x.InnerText));

    foreach (var track in tracks)
    {
        Console.WriteLine($"Adding track: {track}");

        var searchRequest = new SearchRequest(SearchRequest.Types.Track, track);
        var searchResponse = await spotify.Search.Item(searchRequest);

        if (searchResponse.Tracks.Items != null && searchResponse.Tracks.Items.Any())
        {
            var trackUris = new PlaylistAddItemsRequest(new List<string>() { searchResponse.Tracks.Items.First().Uri });
            var addResponse = await spotify.Playlists.AddItems(spotifyPlaylist.Id, trackUris);
        }
    }
}

static string NewPlaylistName()
{
    var now = DateTime.UtcNow;
    var newPlaylistName = $"WebPlaylist-{now.ToShortMonthName()}-{now.Year}";
    return newPlaylistName;
}

static async Task<FullPlaylist> CreatePlaylist(AppConfig appConfig, SpotifyClient spotify)
{
    var newPlaylistName = NewPlaylistName();
    var playlistCresteRequest = new PlaylistCreateRequest(newPlaylistName);
    var playlist = await spotify.Playlists.Create(appConfig.SpotifyUsername, playlistCresteRequest);

    Console.WriteLine($"Created spotify playlist: {newPlaylistName}");
    return playlist;
}