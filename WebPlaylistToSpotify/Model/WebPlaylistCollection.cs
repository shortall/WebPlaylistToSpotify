namespace WebPlaylistToSpotify.Model
{
    internal class WebPlaylistCollection
    {
        public string Name { get; set; } = String.Empty;

        public WebPlaylist[]? Playlists { get; }
    }
}
