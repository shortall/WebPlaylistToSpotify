using System.ComponentModel.DataAnnotations;

namespace WebPlaylistToSpotify.Model
{
    public sealed class WebPlaylist
    {
        [Url]
        public string? Url { get; set; }

        [Required]
        public string? TrackNamesXPath { get; set; }
    }
}
