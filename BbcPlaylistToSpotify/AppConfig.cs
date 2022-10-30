using Microsoft.Extensions.Configuration;

namespace BbcPlaylistToSpotify
{
    internal class AppConfig
    {
        public string SpotifyUsername { get; set; }
        public string SpotifyApiToken { get; set; }
        public string[] BbcPlaylistUrls { get; set; }

        internal static AppConfig Get()
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

            var config = configuration.Build();
            var appConfig = new AppConfig();
            config.Bind(appConfig);
            appConfig.Validate();

            return appConfig;
        }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(SpotifyUsername))
            {
                throw new InvalidOperationException("SpotifyUsername not configured");
            }

            if (string.IsNullOrWhiteSpace(SpotifyApiToken))
            {
                throw new InvalidOperationException("SpotifyApiToken not configured");
            }

            if (!BbcPlaylistUrls?.Any() == true)
            {
                throw new InvalidOperationException("BbcPlaylistUrls not configured");
            }
        }
    }
}
