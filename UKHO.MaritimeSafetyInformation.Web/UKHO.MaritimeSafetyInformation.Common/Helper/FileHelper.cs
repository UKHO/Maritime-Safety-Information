using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.MaritimeSafetyInformation.Common.Helper
{
    public static class FileHelper
    {
        private static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
    }
}
