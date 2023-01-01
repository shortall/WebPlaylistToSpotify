using System.Xml.XPath;

namespace WebPlaylistToSpotify.Model
{
    internal class WebPlaylist
    {
        public string? Url { get; set; }
        public string? TrackNamesXPath { get; set; }

        internal IEnumerable<string> Validate(IEnumerable<string> errors)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                errors.Append("Web playlist Url not missing");
            }

            if (string.IsNullOrWhiteSpace(TrackNamesXPath))
            {
                errors.Append("Web playlist TrackNamesXPath not missing");
            }
            else
            {
                try
                {
                    XPathExpression.Compile(TrackNamesXPath);
                }
                catch (XPathException ex)
                {
                    errors.Append($"XPath syntax error: {ex.Message}");
                }
            }

            return errors;
        }
    }
}
