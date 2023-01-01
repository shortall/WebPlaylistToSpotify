using Microsoft.Extensions.Configuration;
using System.Text;

namespace WebPlaylistToSpotify
{
    internal class AppConfig
    {
        private static string SettingsFileName = "appsettings.json";
        private static string DevSettingsFileName = "appsettings.local.json";

        public string SpotifyUsername { get; set; }
        public string SpotifyApiToken { get; set; }
        public string[] WebPlaylistUrls { get; set; }

        public AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                .AddJsonFile(DevSettingsFileName, optional: true, reloadOnChange: true);

            var config = configuration.Build();

            SpotifyUsername = config.GetValue<string>("SpotifyUsername");
            SpotifyApiToken = config.GetValue<string>("SpotifyApiToken");
            WebPlaylistUrls = config.GetSection("WebPlaylistUrls").Get<string[]>();

            Validate();
        }

        internal void Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(SpotifyUsername))
            {
                errors.Add("SpotifyUsername not configured");
            }

            if (string.IsNullOrWhiteSpace(SpotifyApiToken))
            {
                errors.Add("SpotifyApiToken not configured");
            }

            if (!WebPlaylistUrls?.Any() == true)
            {
                errors.Add("WebPlaylistUrls not configured");
            }

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
