namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (number / 1024 >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{Convert.ToInt32(Math.Round(number))}{suffixes[counter]}";
        }
    }
}
