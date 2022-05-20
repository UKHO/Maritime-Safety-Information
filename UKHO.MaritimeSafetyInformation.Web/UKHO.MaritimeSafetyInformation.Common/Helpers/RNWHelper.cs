namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public static class RnwHelper
    {
        public static string FormatContent(string content)
        {
            return string.IsNullOrEmpty(content)
               ? string.Empty
               : content.Length > 300 ? string.Concat(content[..300], "...") : content;
        }
    }
}
