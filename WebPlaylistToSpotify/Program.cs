#pragma warning disable CA1852

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    var serviceCollection = new ServiceCollection();

    serviceCollection.AddOptions<AppConfig>()
           .Bind(configRoot)
           .ValidateDataAnnotations();

    var transformer = serviceCollection
        .AddSingleton<IValidateOptions<AppConfig>, AppConfigValidator>()
        .AddSingleton<PlaylistTransformer>()
        .BuildServiceProvider()
        .GetRequiredService<PlaylistTransformer>();

    await transformer.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Console.WriteLine($"Hit <Enter> to finish");
    Console.ReadLine();
}