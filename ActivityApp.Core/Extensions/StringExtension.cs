using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ActivityApp.Core.Extensions
{
    public static class StringExtension
    {

        /// <summary>
        /// SkipAfter skips the string value after provided string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueToSkip"></param>
        /// <returns></returns>
        public static string SkipAfter(this string value, string valueToSkip)
        {
            if (value.Contains(valueToSkip, StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.IndexOf(valueToSkip, StringComparison.OrdinalIgnoreCase) + valueToSkip.Length);
            }

            return value;
        }
    }
}
