namespace WebPlaylistToSpotify.Model
{
    internal class AppConfig
    {
        public string SpotifyUsername { get; set; } = string.Empty;
        public string SpotifyClientId { get; set; } = string.Empty;
        public List<WebPlaylist> WebPlaylists { get; set; } = new List<WebPlaylist>();
    }
}