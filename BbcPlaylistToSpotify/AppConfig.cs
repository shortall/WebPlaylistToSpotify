using Microsoft.Extensions.Configuration;
using System.Text;

namespace BbcPlaylistToSpotify
{
    internal class AppConfig
    {
        private static string SettingsFileName = "appsettings.json";
        private static string DevSettingsFileName = "appsettings.local.json";

        public string SpotifyUsername { get; set; }
        public string SpotifyApiToken { get; set; }
        public string[] BbcPlaylistUrls { get; set; }

        public AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                .AddJsonFile(DevSettingsFileName, optional: true, reloadOnChange: true);

            var config = configuration.Build();

            SpotifyUsername = config.GetValue<string>("SpotifyUsername");
            SpotifyApiToken = config.GetValue<string>("SpotifyApiToken");
            BbcPlaylistUrls = config.GetValue<string[]>("BbcPlaylistUrls");

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

            if (!BbcPlaylistUrls?.Any() == true)
            {
                errors.Add("BbcPlaylistUrls not configured");
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
