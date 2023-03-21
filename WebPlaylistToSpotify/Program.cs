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

    var configRoot = configBuilder.Build();
    var appConfigSection = configRoot.GetSection("AppConfig");
    var serviceCollection = new ServiceCollection();

    var transformer = serviceCollection
        .Configure<AppConfig>(appConfigSection)
        .AddSingleton<PlaylistTransformer, PlaylistTransformer>()
        .BuildServiceProvider()
        .GetService<PlaylistTransformer>();

    if (transformer == null)
    {
        throw new InvalidOperationException("Failed to configure service collection");
    }

    await transformer.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Console.WriteLine($"Hit <Enter> to finish");
    Console.ReadLine();
}