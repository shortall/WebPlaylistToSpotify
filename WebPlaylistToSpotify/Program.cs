using WebPlaylistToSpotify.Extensions;
using HtmlAgilityPack;
using SpotifyAPI.Web;
using System.Web;
using WebPlaylistToSpotify.Model;

try
{
    var appConfig = AppConfig.Get();
    var spotify = new SpotifyClient(appConfig.SpotifyApiToken);

    Console.WriteLine("Starting...");

    var spotifyPlaylist = await CreatePlaylist(appConfig, spotify);
    await AddTracks(appConfig, spotify, spotifyPlaylist);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Console.WriteLine($"Hit <Enter> to finish");
    Console.ReadLine();
}

static async Task AddWebPlaylist(SpotifyClient spotify, FullPlaylist playlist, string WebPlaylistHtml, string trackNamesXPath)
{
    var doc = new HtmlDocument();
    doc.LoadHtml(WebPlaylistHtml);

    var tracks = doc.DocumentNode
        .SelectNodes(trackNamesXPath)
        .Select(x => HttpUtility.HtmlDecode(x.InnerText));

    foreach (var track in tracks)
    {
        Console.WriteLine($"Adding track: {track}");

        var searchRequest = new SearchRequest(SearchRequest.Types.Track, track);
        var searchResponse = await spotify.Search.Item(searchRequest);
        var trackUris = new PlaylistAddItemsRequest(new List<string>() { searchResponse.Tracks.Items.First().Uri });
        var addResponse = await spotify.Playlists.AddItems(playlist.Id, trackUris);
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