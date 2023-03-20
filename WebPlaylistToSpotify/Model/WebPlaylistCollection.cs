namespace WebPlaylistToSpotify.Model
{
    internal class WebPlaylistCollection
    {
        public string Name { get; set; } = String.Empty;

        public List<WebPlaylist> Playlists { get; set; } = new List<WebPlaylist>();
    }
}
