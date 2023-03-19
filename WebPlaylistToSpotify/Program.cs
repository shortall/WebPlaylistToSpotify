using HtmlAgilityPack;
using SpotifyAPI.Web;
using System.Web;
using WebPlaylistToSpotify.Auth;
using WebPlaylistToSpotify.Extensions;
using WebPlaylistToSpotify.Model;

try
{
    var appConfig = new AppConfig();

    var browser = new SystemBrowser(appConfig.SpotifyClientId);
    var queryString = await browser.InvokeAsync();
    var parsedQuery = HttpUtility.ParseQueryString(queryString);
    var token = await browser.GetToken(parsedQuery["code"]);

    var spotify = new SpotifyClient(token);

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