using System;

namespace ActivityApp.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal RoundToNearest(this decimal value)
        {
            return Math.Round(value, 0);
        }

        public static decimal RoundDecimalTo(this decimal value, int decimalValue = 2)
        {
            return Math.Round(value, decimalValue);
        }
    }
}
