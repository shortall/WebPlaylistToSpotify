using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using System.Web;
using WebPlaylistToSpotify.Auth;
using WebPlaylistToSpotify.Model;

namespace WebPlaylistToSpotify
{
    internal sealed class PlaylistTransformer
    {
        private readonly AppConfig _appConfig;
        public PlaylistTransformer(IOptions<AppConfig> appConfigOptions)
        {
            _appConfig = appConfigOptions.Value;
        }

        public async Task Run()
        {
            try
            {
                var token = await GetAuthToken(_appConfig);
                var spotify = new SpotifyClient(token);

                Console.WriteLine("Starting...");

                var playlist = await CreatePlaylist(_appConfig, spotify);
                await AddTracks(_appConfig, spotify, playlist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task AddTracks(AppConfig appConfig, SpotifyClient spotify, FullPlaylist spotifyPlaylist)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var webPlaylist in appConfig.WebPlaylistConfig.Playlists)
                {
                    Console.WriteLine($"Downloading playlist: {webPlaylist.Url}");
                    var html = await httpClient.GetStringAsync(webPlaylist.Url);

                    await AddWebPlaylist(spotify, spotifyPlaylist, html, webPlaylist.TrackNamesXPath);
                }

                Console.WriteLine("Done");
            }
        }

        private static async Task AddWebPlaylist(SpotifyClient spotify, FullPlaylist spotifyPlaylist, string webPlaylistHtml, string? trackNamesXPath)
        {
            if (spotifyPlaylist?.Id == null)
            {
                throw new ArgumentNullException(nameof(spotifyPlaylist));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(webPlaylistHtml);

            var tracks = doc.DocumentNode
                .SelectNodes(trackNamesXPath)
                .Select(x => HttpUtility.HtmlDecode(x.InnerText))
                .Where(t => !string.IsNullOrWhiteSpace(t));

            foreach (var track in tracks)
            {
                Console.WriteLine($"Adding track: {track}");

                var searchRequest = new SearchRequest(SearchRequest.Types.Track, track);
                var searchResponse = await spotify.Search.Item(searchRequest);

                if (searchResponse.Tracks.Items != null && searchResponse.Tracks.Items.Any())
                {
                    var trackUris = new PlaylistAddItemsRequest(new List<string>() { searchResponse.Tracks.Items.First().Uri });
                    await spotify.Playlists.AddItems(spotifyPlaylist.Id, trackUris);
                }
            }
        }

        private static async Task<FullPlaylist> CreatePlaylist(AppConfig appConfig, SpotifyClient spotify)
        {
            var newPlaylistName = appConfig.WebPlaylistConfig.GenerateName();
            var playlistCresteRequest = new PlaylistCreateRequest(newPlaylistName);
            var playlist = await spotify.Playlists.Create(appConfig.SpotifyUsername, playlistCresteRequest);

            Console.WriteLine($"Created spotify playlist: {newPlaylistName}");
            return playlist;
        }

        private static async Task<string> GetAuthToken(AppConfig appConfig)
        {
            var browser = new SpotifyPkceBrowser(appConfig.SpotifyClientId);
            return await browser.Authorise();
        }
    }
}