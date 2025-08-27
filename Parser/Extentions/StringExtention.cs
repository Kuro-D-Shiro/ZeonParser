using System.Text.RegularExpressions;

namespace ZeonService.Parser.Extentions
{
    public static class StringExtention
    {
        public static Decimal ParsePriceFromString(this string priceString)
        {
            try
            {
                return (Decimal)Convert.ChangeType(Regex.Replace(priceString, @"[^0-9\,\.]", "").Replace(",", "."), typeof(Decimal));
            }
            catch (FormatException ex)
            {
                return default; //как будто бы лучше ошибку пробросить дальше
            }
        }
    }
}
