using System;
using System.Globalization;

namespace Utility
{
    public static class NumberFormatter
    {
        public static string FormatNumberWithCommas(int number)
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            NumberFormatInfo numberFormat = currentCulture.NumberFormat;

            string formattedNumber = number.ToString("N0", numberFormat);
            return formattedNumber;
        }

        public static string FormatCleanTime(float value)
        {
            var time = TimeSpan.FromSeconds(value);
            return time.Minutes <= 0
                ? $"{time.Seconds}s"
                : $"{time.Minutes}m{time.Seconds}s";
        }
    }
}