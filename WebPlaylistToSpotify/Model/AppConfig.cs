using Microsoft.Extensions.Configuration;
using System.Text;

namespace WebPlaylistToSpotify.Model
{
    internal class AppConfig
    {
        private static string SettingsFileName = "appsettings.json";
        private static string DevSettingsFileName = "appsettings.local.json";

        public string SpotifyUsername { get; set; }
        public string SpotifyApiToken { get; set; }
        public WebPlaylistCollection WebPlaylistCollection { get; set; }

        public AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                .AddJsonFile(DevSettingsFileName, optional: true, reloadOnChange: true);

            var config = configuration.Build();

            SpotifyUsername = config.GetValue<string>("SpotifyUsername");
            SpotifyApiToken = config.GetValue<string>("SpotifyApiToken");
            WebPlaylistCollection = config.GetSection("WebPlaylistCollection").Get<WebPlaylistCollection>();

            Validate();
        }

        internal void Validate()
        {
            IEnumerable<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(SpotifyUsername))
            {
                errors.Append("SpotifyUsername not configured");
            }

            if (string.IsNullOrWhiteSpace(SpotifyApiToken))
            {
                errors.Append("SpotifyApiToken not configured");
            }

            if (WebPlaylistCollection == null || WebPlaylistCollection.WebPlaylists == null || !WebPlaylistCollection.WebPlaylists.Any())
            {
                errors.Append("WebPlaylists are configured");
            }
            else
            {
                foreach (var webPlaylist in WebPlaylistCollection.WebPlaylists)
                {
                    errors = webPlaylist.Validate(errors);
                }
            }

            HandleErrors(errors);
        }

        private static void HandleErrors(IEnumerable<string> errors)
        {
            if (errors.Any())
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Error in config, see {SettingsFileName}");

                foreach (var error in errors)
                {
                    sb.AppendLine(error);
                }

                throw new InvalidOperationException(sb.ToString());
            }
        }
    }
}