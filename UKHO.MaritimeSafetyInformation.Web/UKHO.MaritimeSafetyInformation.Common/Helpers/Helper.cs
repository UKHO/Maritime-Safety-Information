namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public static class RnwHelper
    {
        public static string FormatContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return content.Length > 300 ? string.Concat(content.Substring(0, 300), "...") : content;
            }
            return string.Empty;
        }
    }
}
