using System.Globalization;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRnwDateFormat(this DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;
            return dateTime?.ToString("ddhhmm UTC MMM yy", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
