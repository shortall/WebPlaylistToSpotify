using WebPlaylistToSpotify.Extensions;

namespace WebPlaylistToSpotify.Model
{
    internal sealed class WebPlaylistCollection
    {
        public string Name { get; set; } = String.Empty;

        public List<WebPlaylist> Playlists { get; set; } = new List<WebPlaylist>();

        internal string GenerateName()
        {
            var now = DateTime.UtcNow;
            var newPlaylistName = $"{Name}-{now.ToShortMonthName()}-{now.Year}";
            return newPlaylistName;
        }
    }
}
