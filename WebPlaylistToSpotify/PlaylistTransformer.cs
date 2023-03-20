﻿using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using System.Web;
using WebPlaylistToSpotify.Auth;
using WebPlaylistToSpotify.Extensions;
using WebPlaylistToSpotify.Model;

namespace WebPlaylistToSpotify
{
    internal class PlaylistTransformer
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
                string token = await GetAuthToken(_appConfig);
                var spotify = new SpotifyClient(token);

                Console.WriteLine("Starting...");

                var playlist = await CreatePlaylist(_appConfig, spotify);
                await AddTracks(_appConfig, spotify, playlist);
            }
            catch (Exception ex) 
            {
                var a = 1;
            }
        }

        private async Task AddTracks(AppConfig appConfig, SpotifyClient spotify, FullPlaylist spotifyPlaylist)
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

        private async Task AddWebPlaylist(SpotifyClient spotify, FullPlaylist spotifyPlaylist, string webPlaylistHtml, string? trackNamesXPath)
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

        private string NewPlaylistName()
        {
            var now = DateTime.UtcNow;
            var newPlaylistName = $"WebPlaylist-{now.ToShortMonthName()}-{now.Year}";
            return newPlaylistName;
        }

        private async Task<FullPlaylist> CreatePlaylist(AppConfig appConfig, SpotifyClient spotify)
        {
            var newPlaylistName = NewPlaylistName();
            var playlistCresteRequest = new PlaylistCreateRequest(newPlaylistName);
            var playlist = await spotify.Playlists.Create(appConfig.SpotifyUsername, playlistCresteRequest);

            Console.WriteLine($"Created spotify playlist: {newPlaylistName}");
            return playlist;
        }

        private async Task<string> GetAuthToken(AppConfig appConfig)
        {
            var browser = new SpotifyOAuthBrowser(appConfig.SpotifyClientId);
            var resultQueryString = await browser.StartFlow();
            var parsedQuery = HttpUtility.ParseQueryString(resultQueryString);

            var code = parsedQuery["code"];

            if (code == null)
            {
                throw new InvalidOperationException("Missing authorisation code");
            }

            var token = await browser.GetToken(code);
            return token;
        }
    }
}