using System.Globalization;

namespace WebPlaylistToSpotify.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
