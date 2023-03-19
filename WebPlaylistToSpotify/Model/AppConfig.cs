using Microsoft.Extensions.Configuration;
using System.Text;

namespace WebPlaylistToSpotify.Model
{
    internal class AppConfig
    {
        private static string SettingsFileName = "appsettings.json";
        private static string DevSettingsFileName = "appsettings.local.json";

        public string SpotifyUsername { get; set; }
        public string SpotifyClientId { get; set; }
        public WebPlaylist[] WebPlaylists { get; set; }

        public AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                .AddJsonFile(DevSettingsFileName, optional: true, reloadOnChange: true);

            var config = configuration.Build();

            SpotifyUsername = config.GetValue<string>("SpotifyUsername") ?? string.Empty;
            SpotifyClientId = config.GetValue<string>("SpotifyClientId") ?? string.Empty;
            WebPlaylists = config.GetSection("WebPlaylists").Get<WebPlaylist[]>() ?? new WebPlaylist[0];

            Validate();
        }

        internal void Validate()
        {
            IList<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(SpotifyUsername))
            {
                errors.Add("SpotifyUsername not configured");
            }

            if (string.IsNullOrWhiteSpace(SpotifyClientId))
            {
                errors.Add("SpotifyClientId not configured");
            }

            if (WebPlaylists == null || !WebPlaylists.Any())
            {
                errors.Add("WebPlaylists are configured");
            }
            else
            {
                foreach (var webPlaylist in WebPlaylists)
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