namespace WebPlaylistToSpotify.Model
{
    internal sealed class AppConfig
    {
        public string SpotifyUsername { get; set; } = string.Empty;
        public string SpotifyClientId { get; set; } = string.Empty;
        public WebPlaylistCollection WebPlaylistCollection { get; set; } = new WebPlaylistCollection();
    }
}