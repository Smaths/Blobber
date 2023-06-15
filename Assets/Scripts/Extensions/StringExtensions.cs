using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringExtensions
    {
        public static string SplitCamelCase( this string str )
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        /// <summary>
        /// Convert string containing color HEX code to color object.
        /// <para>(example: <code>#FFFFFF".ToColor()</code>)</para>.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color ToColor(this string hex) {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = byte.Parse(hex.Substring(0,2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2,2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4,2), NumberStyles.HexNumber);

            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6,2), NumberStyles.HexNumber);

            return new Color32(r,g,b,a);
        }
    }
}