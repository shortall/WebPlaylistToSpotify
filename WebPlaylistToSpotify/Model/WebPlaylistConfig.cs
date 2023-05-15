using System.ComponentModel.DataAnnotations;
using WebPlaylistToSpotify.Attributes;
using WebPlaylistToSpotify.Extensions;

namespace WebPlaylistToSpotify.Model
{
    public sealed class WebPlaylistConfig
    {
        [Required]
        public string Name { get; set; } = String.Empty;

        [Contains(ErrorMessage = "At least one playlist required")]
        public List<WebPlaylist> Playlists { get; set; } = new List<WebPlaylist>();

        internal string GenerateName()
        {
            var now = DateTime.UtcNow;
            var newPlaylistName = $"{Name}-{now.ToShortMonthName()}-{now.Year}";
            return newPlaylistName;
        }
    }
}
