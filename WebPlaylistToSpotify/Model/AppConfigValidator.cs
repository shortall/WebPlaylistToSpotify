using Microsoft.Extensions.Options;
using System.Xml.XPath;

namespace WebPlaylistToSpotify.Model
{
    public class AppConfigValidator : IValidateOptions<AppConfig>
    {
        public ValidateOptionsResult Validate(string? name, AppConfig options)
        {
            foreach (var xPath in options.WebPlaylistConfig.Playlists.Select(p => p.TrackNamesXPath)) 
            {
                try
                {
                    XPathExpression.Compile(xPath!);
                }
                catch (XPathException ex) 
                {
                    return ValidateOptionsResult.Fail($"Bad XPath {xPath}. {ex.Message}");
                }
            }

            return ValidateOptionsResult.Success;
        }
    }
}
