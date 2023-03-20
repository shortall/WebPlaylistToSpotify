using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebPlaylistToSpotify;
using WebPlaylistToSpotify.Model;

try
{
    var settingsFileName = "appsettings.json";
    var devSettingsFileName = "appsettings.local.json";

    var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsFileName, optional: false, reloadOnChange: true)
                    .AddJsonFile(devSettingsFileName, optional: true, reloadOnChange: true);

    var config = configBuilder.Build();
    var services = new ServiceCollection();

    await services
        .Configure<AppConfig>(config.GetSection("AppConfig"))
        .AddSingleton<PlaylistTransformer, PlaylistTransformer>()
        ?.BuildServiceProvider()
        ?.GetService<PlaylistTransformer>()
        ?.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Console.WriteLine($"Hit <Enter> to finish");
    Console.ReadLine();
}