using System.Globalization;
using System.Text.RegularExpressions;

namespace ZeonService.Parser.Extentions
{
    public static class StringExtention
    {
        public static decimal? ParsePriceFromString(this string priceString)
        {
            if (string.IsNullOrWhiteSpace(priceString))
                return null;

            string cleanPrice = Regex.Replace(priceString, @"[^0-9\,\.]", "").Replace(",", ".");

            if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            return null;
        }
    }
}
