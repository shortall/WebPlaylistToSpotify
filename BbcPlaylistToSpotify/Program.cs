using HtmlAgilityPack;
using SpotifyAPI.Web;
using BbcPlaylistToSpotify;
using BbcPlaylistToSpotify.Extensions;

var appConfig = AppConfig.Get();
var spotify = new SpotifyClient(appConfig.SpotifyApiToken);

var playlist = await CreatePlaylist(appConfig, spotify);
await AddTracks(appConfig, spotify, playlist);


static async Task AddBbcPlaylist(SpotifyClient spotify, FullPlaylist playlist, string bbcPlaylistHtml)
{
    var doc = new HtmlDocument();
    doc.LoadHtml(bbcPlaylistHtml);

    var tracks = doc.DocumentNode
        .SelectNodes("//div[@class='text--prose']/p")
        .Where(x => x.LastChild.Name == "#text" || x.LastChild.Name == "strong")
        .Select(x => x.InnerText);

    foreach (var track in tracks)
    {
        var searchRequest = new SearchRequest(SearchRequest.Types.Track, track);
        var searchResponse = await spotify.Search.Item(searchRequest);
        var trackUris = new PlaylistAddItemsRequest(new List<string>() { searchResponse.Tracks.Items.First().Uri });
        var addResponse = await spotify.Playlists.AddItems(playlist.Id, trackUris);
    }
}

static string NewPlaylistName()
{
    var now = DateTime.UtcNow;
    var newPlaylistName = $"NewPlaylist-{now.ToShortMonthName()}-{now.Year}";
    return newPlaylistName;
}

static async Task<FullPlaylist> CreatePlaylist(AppConfig appConfig, SpotifyClient spotify)
{
    var newPlaylistName = NewPlaylistName();
    var playlistCresteRequest = new PlaylistCreateRequest(newPlaylistName);
    var playlist = await spotify.Playlists.Create(appConfig.SpotifyUsername, playlistCresteRequest);
    return playlist;
}

static async Task AddTracks(AppConfig appConfig, SpotifyClient spotify, FullPlaylist playlist)
{
    using (var httpClient = new HttpClient())
    {
        Console.WriteLine("Starting...");

        foreach (var bbcPlaylistUrl in appConfig.BbcPlaylistUrls)
        {
            await AddBbcPlaylist(spotify, playlist, bbcPlaylistUrl);
        }

        Console.WriteLine("Done");
    }
}