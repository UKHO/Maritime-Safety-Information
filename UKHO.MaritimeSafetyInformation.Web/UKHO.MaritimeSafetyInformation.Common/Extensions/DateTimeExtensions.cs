using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DateTimeExtensions
    {
        public static string ToRnwDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString("ddhhmm UTC mmm yy", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
