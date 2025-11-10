using System;
using System.Text.RegularExpressions;

namespace Parsing_Jrn_Ej.Helpers
{
    public static class ParsingHelper
    {
        public static int? ExtractInt(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        public static decimal? ExtractDecimal(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            if (!match.Success) return null;

            var value = match.Groups[1].Value.Replace(".", "").Replace(",", ".");
            return decimal.TryParse(value, out var result) ? result : null;
        }

        public static DateTime? ExtractDateTime(string text)
        {
            var match = Regex.Match(text, @"(\d{2}/\d{2}/\d{4})\s+(\d{2}:\d{2}:\d{2})");
            if (!match.Success) return null;

            string dateString = $"{match.Groups[1].Value} {match.Groups[2].Value}";
            string[] possibleFormats = { "MM/dd/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };

            if (DateTime.TryParseExact(dateString, possibleFormats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return parsedDate; // jangan ubah timezone
            }

            Console.WriteLine($"⚠️ Format tanggal tidak dikenal: {dateString}");
            return null;
        }

        public static string? ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        public static string DetectJenisFile(string fileName)
        {
            if (fileName.Contains("HTC", StringComparison.OrdinalIgnoreCase)) return "HTC";
            if (fileName.Contains("WIN", StringComparison.OrdinalIgnoreCase)) return "WIN";
            if (fileName.Contains("HYS", StringComparison.OrdinalIgnoreCase)) return "HYS";
            return "UNKNOWN";
        }

        public static string ExtractJenisTransaksi(string text)
        {
            if (Regex.IsMatch(text, "SETOR", RegexOptions.IgnoreCase)) return "Setor Tunai";
            if (Regex.IsMatch(text, "TARIK", RegexOptions.IgnoreCase)) return "Tarik Tunai";
            if (Regex.IsMatch(text, "SALDO", RegexOptions.IgnoreCase)) return "Cek Saldo";
            return "Lainnya";
        }
    }
}
