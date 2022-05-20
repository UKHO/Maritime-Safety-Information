using System.Globalization;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRnwDateFormat(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty: dateTime?.ToString("ddhhmm UTC MMM yy", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
