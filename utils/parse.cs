using System;
using System.Text.RegularExpressions;

namespace parsing_Jrn_Ej.Helpers
{
    public class ParsingHelper
    {
        public string DetectNamaATM(string fileName)
        {
            if (fileName.Contains("HTC", StringComparison.OrdinalIgnoreCase)) return "HTC";
            if (fileName.Contains("WIN", StringComparison.OrdinalIgnoreCase)) return "WIN";
            if (fileName.Contains("HYS", StringComparison.OrdinalIgnoreCase)) return "HYS";
            return "UNKNOWN";
        }

        public string DetectJenisFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".txt")
                return "txt";
            if (extension == ".jrn")
                return "jrn";

            try
            {
                string firstLine = File.ReadLines(filePath).FirstOrDefault()?.ToLower() ?? "";
                if (firstLine.Contains("transaction start"))
                    return "Kemungkinan File Jurnal (.jrn berdasarkan isi)";
            }
            catch { }

            return $"Tipe file tidak dikenal ({extension})";
        }



        public string? ExtractTerminalId(string text)
        {
            string[] patterns = new[]
            {
                @"TERMINAL\s*[:\-]?\s*([A-Z]{2,}[0-9]{3,10})",
                @"Terminal\s*Id\s*[:\-]?\s*([A-Z]{2,}[0-9]{3,10})",

                @"ATM[^\r\n]*[\r\n]+\s*([A-Z]{2,}[0-9]{3,10})",

                @"(HTC|WIN|HYS|TER|ATM)[\-_ ]?([0-9]{3,10})"
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (m.Success)
                {
                    if (m.Groups.Count > 2)
                        return (m.Groups[1].Value + m.Groups[2].Value).ToUpperInvariant();
                    return m.Groups[1].Value.ToUpperInvariant();
                }
            }

            return null;
        }


        public string? ExtractAtmCode(string text)
        {
            string[] patterns = new[]
            {
                @"ATM\s*[:\-]?\s*([A-Z]{2,}[0-9]{3,10})",

                @"ATM[^\r\n]*[\r\n]+\s*([A-Z]{2,}[0-9]{3,10})",

                @"(WIN|HTC|HYS)[\-_ ]?([0-9]{3,10})"
            };

            foreach (var p in patterns)
            {
                var m = Regex.Match(text, p, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (m.Success)
                {
                    if (m.Groups.Count > 2)
                        return (m.Groups[1].Value + m.Groups[2].Value).ToUpperInvariant();
                    return m.Groups[1].Value.ToUpperInvariant();
                }
            }

            return null;
        }





        public string? ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!match.Success) return null;

            if (match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(match.Groups[2].Value))
                return match.Groups[2].Value.Trim();

            return match.Groups[1].Value.Trim();
        }


        public int? ExtractInt(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        public decimal? ExtractDecimal(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            if (!match.Success) return null;

            var digitsOnly = Regex.Replace(match.Groups[1].Value, @"[^\d]", "");

            if (string.IsNullOrEmpty(digitsOnly)) return null;

            if (decimal.TryParse(digitsOnly, out var result))
                return result;

            return null;
        }



        public DateTime? ExtractDateTime(string text)
        {
            // Support tahun 2 digit / 4 digit
            var match = Regex.Match(text, @"(\d{2}/\d{2}/\d{2,4})\s+(\d{2}:\d{2}:\d{2})");
            if (!match.Success) return null;

            string dateString = $"{match.Groups[1].Value} {match.Groups[2].Value}";

            string[] possibleFormats =
            {
        "MM/dd/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss",
        "MM/dd/yy HH:mm:ss",
        "dd/MM/yy HH:mm:ss"
    };

            if (DateTime.TryParseExact(dateString, possibleFormats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return parsedDate;
            }

            Console.WriteLine($"⚠️ Format tanggal tidak dikenal: {dateString}");
            return null;
        }

        public string? ExtractPesanError(string text)
        {
            var lines = text.Split('\n')
                            .Select(l => l.TrimEnd('\r'))
                            .ToList();

            int index = lines.FindIndex(l =>
                Regex.IsMatch(l, @"^(PESAN|MESSAGE)\s*:", RegexOptions.IgnoreCase)
            );

            if (index == -1) return null;

            var pesanLines = new List<string>();

            for (int i = index + 1; i < lines.Count && i <= index + 2; i++)
            {
                var clean = lines[i].Trim();
                if (!string.IsNullOrWhiteSpace(clean))
                    pesanLines.Add(clean);
            }

            pesanLines = pesanLines
                .Where(l => !Regex.IsMatch(l, @"^\s*X+\s*$"))
                .Where(l => !Regex.IsMatch(l, @"^\d{2}:\d{2}:\d{2}"))
                .Where(l => !Regex.IsMatch(l, @"CARD\(", RegexOptions.IgnoreCase))
                .Where(l => !Regex.IsMatch(l, @"TAKEN", RegexOptions.IgnoreCase))
                .ToList();

            if (pesanLines.Count == 0)
                return null;

            string pesanRaw = string.Join(" ", pesanLines).Trim();
            pesanRaw = Regex.Replace(pesanRaw, @"\sX+$", "").Trim();


            var specialErrors = new[]
            {
        "MAAF SEMENTARA TRANSAKSI",
        "SORRY CAN NOT BE DONE",
        "FAILED COMMUNICATION"
    };

            bool isSpecialError = specialErrors.Any(err =>
                pesanRaw.Contains(err, StringComparison.OrdinalIgnoreCase)
            );

            if (isSpecialError)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    var above = lines[i].Trim();

                    if (string.IsNullOrWhiteSpace(above))
                        continue;

                    if (Regex.IsMatch(above, @"NO\s*KARTU", RegexOptions.IgnoreCase))
                        return null;

                    return above;
                }

                return null;
            }



            // Normal fallback rekening tidak valid
            var bankFallbackPattern =
                @"^(BANK\s+)?(BCA|BNI|BRI|BSI|BTN|MANDIRI|CIMB|DANAMON|PERMATA|PANIN|OCBC|MEGA)(\b.*)$";

            if (Regex.IsMatch(pesanRaw, bankFallbackPattern, RegexOptions.IgnoreCase))
            {
                return "Rekening tujuan tidak valid atau data tidak lengkap";
            }

            return string.IsNullOrEmpty(pesanRaw) ? null : pesanRaw;
        }









        public string? ExtractJenisTransaksi(string text)
        {
            if (text.Contains("PENARIKAN", StringComparison.OrdinalIgnoreCase)) return "Tarik Tunai";
            if (text.Contains("SETOR", StringComparison.OrdinalIgnoreCase)) return "Setor Tunai";
            if (text.Contains("LISTRIK", StringComparison.OrdinalIgnoreCase)) return "Pembelian Listrik";
            if (text.Contains("CEK SALDO", StringComparison.OrdinalIgnoreCase)) return "Cek Saldo";
            return "Lainnya";
        }
    }
}
