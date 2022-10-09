using System.Globalization;

namespace BbcPlaylistToSpotify.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
