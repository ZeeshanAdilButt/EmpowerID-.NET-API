using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ActivityApp.Core.Extensions
{
    public static class ConvertExtensions
    {
        /// <summary>
        /// ConvertWeight converts weight string to an integer weight by removing the units
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNumbersFromString(this string value)
        {
            string number = value.ToLower();

            number = Regex.Match(value, @"\d+.+\d").Value;

            if (string.IsNullOrWhiteSpace(number))
                return null;

            return number;
        }

        /// <summary>
        /// ConvertWeight converts weight string to an integer weight by removing the units
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetAlphaCharactersFromString(this string value)
        {
            string alphabets = value.ToLower();

            alphabets = Regex.Match(value, @"[a-z]+", RegexOptions.IgnoreCase).Value;

            if (string.IsNullOrWhiteSpace(alphabets))
                return null;

            return alphabets;
        }

        /// <summary>
        /// ConvertWeight converts weight string to an integer weight by removing the units
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasAlphaCharacter(this string value)
        {
            if (value.GetAlphaCharactersFromString() == null)
                return false;

            return true;
        }

        public static int ReturnOneIfNullOrZero(this int? value)
        {
            if (value == null)
                return 1;

            if (value != null && value == 0)
                return 1;

            return value.Value;
        }

        /// <summary>
        /// IsDigitsOnly checks if the characters are only digits
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

    }
}
