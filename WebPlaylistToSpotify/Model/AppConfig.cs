using System.ComponentModel.DataAnnotations;

namespace WebPlaylistToSpotify.Model
{
    public sealed class AppConfig
    {
        [Required]
        public string SpotifyUsername { get; set; } = string.Empty;

        [Required]
        [StringLength(32)]
        public string SpotifyClientId { get; set; } = string.Empty;

        [Required]
        public WebPlaylistConfig WebPlaylistConfig { get; set; } = new WebPlaylistConfig();
    }
}