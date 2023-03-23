using System.Xml.XPath;

namespace WebPlaylistToSpotify.Model
{
    internal sealed class WebPlaylist
    {
        public string? Url { get; set; }
        public string? TrackNamesXPath { get; set; }

        internal IList<string> Validate(IList<string> errors)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                errors.Add("Web playlist Url not missing");
            }

            if (string.IsNullOrWhiteSpace(TrackNamesXPath))
            {
                errors.Add("Web playlist TrackNamesXPath not missing");
            }
            else
            {
                try
                {
                    XPathExpression.Compile(TrackNamesXPath);
                }
                catch (XPathException ex)
                {
                    errors.Add($"XPath syntax error: {ex.Message}");
                }
            }

            return errors;
        }
    }
}
