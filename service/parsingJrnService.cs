using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using parsing_Jrn_Ej.Data;
using parsing_Jrn_Ej.Models;

namespace parsing_jrn_Ej.Services
{
    public class JrnParserService
    {
        private readonly AppDbContext _context;
        public JrnParserService(AppDbContext context)
        {
            _context = context;
        }
        private readonly string _basePath = "btn_jrn";

        public async Task ProcessPendingFilesAsync()
        {
            string pendingPath = Path.Combine(_basePath, "pending");
            string processedPath = Path.Combine(_basePath, "processed");
            string donePath = Path.Combine(_basePath, "done");
            string errorPath = Path.Combine(_basePath, "error");

            Directory.CreateDirectory(processedPath);
            Directory.CreateDirectory(donePath);
            Directory.CreateDirectory(errorPath);

            foreach (var file in Directory.GetFiles(pendingPath, "*"))
            {
                string fileName = Path.GetFileName(file);
                string processedFile = Path.Combine(processedPath, fileName);

                File.Move(file, processedFile, true);

                try
                {
                    string[] lines = await File.ReadAllLinesAsync(processedFile);

                    bool success = await ParseAndInsertAsync(lines, fileName);
                    if (success)
                    {
                        File.Move(processedFile, Path.Combine(donePath, fileName), true);
                    }
                    else
                    {
                        File.Move(processedFile, Path.Combine(errorPath, fileName), true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing file {fileName}: {ex.Message}");
                    File.Move(processedFile, Path.Combine(errorPath, fileName), true);
                }
            }
        }


        private async Task<bool> ParseAndInsertAsync(string[] lines, string fileName)
        {
            try
            {
                var content = string.Join("\n", lines);
                var blocks = Regex.Matches(content, @"(?is)(transaction start.*?(?:transaction end|<- transaction end|-> transaction end))");

                foreach (Match block in blocks)
                {
                    var text = block.Value;
                    var namaFile = DetectNamaATM(fileName);
                    var pesanError = ExtractPesanError(text);
                    var terminalId = ExtractTerminalId(text) ?? ExtractAtmCode(text);
                    var jenisFile = DetectJenisFile(Path.Combine(_basePath, fileName));

                    var transaksi = new AtmTransaksi
                    {
                        JenisFile = jenisFile,
                        NamaAtm = namaFile,
                        NoTransaksi = ExtractInt(text, @"(\d+)\s+\d{2}/\d{2}/\d{4}"),
                        Waktu = ExtractDateTime(text),
                        NoKartu = ExtractValue(text, @"(?:Card Number|NO KARTU)\s*[:\]]\s*([0-9X*]+)"),
                        JenisTransaksi = ExtractJenisTransaksi(text),
                        TerminalId = terminalId,
                        AtmId = ExtractValue(text, @"ATM ID\s*:([0-9]+)"),
                        Lokasi = ExtractValue(text, @"LOKASI\s*:?(.+)"),
                        OpCode = ExtractValue(text, @"(?:OP Code|TRANSACTION REQUEST).*\[(.*?)\]"),
                        Jumlah = ExtractDecimal(text, @"JUMLAH\s*[:=]\s*RP[.\s]*(\d+[.,]?\d*)"),
                        Saldo = ExtractDecimal(text, @"SALDO\s*[:=]\s*RP[.\s]*(\d+[.,]?\d*)"),
                        NoRef = ExtractValue(text, @"NO\s*REF{1,2}\.?\s*[:=]\s*([0-9]+)"),
                        NoRekening = ExtractValue(text, @"REKENING[:=]\s*([0-9]+)"),
                        PesanError = pesanError,
                        Struk = text,
                        FunctionIdentifier = ExtractValue(text, @"FUNCTION IDENTIFIER\s*[:=]\s*(.*)"),
                        TransSeqNumber = ExtractValue(text, @"TRANSACTION SEQUENCE NUMBER\s*[:=]\s*(.*)"),
                        Tsi = ExtractValue(text, @"TSI\s*[:=]\s*(.*)"),
                        Tvr = ExtractValue(text, @"TVR\s*[:=]\s*([A-Fa-f0-9]{6,})")


                    };

                    _context.AtmTransaksi.Add(transaksi);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Gagal parsing file {fileName}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<AtmTransaksi>> getAllTransaksi()
        {
            return await _context.AtmTransaksi.ToListAsync();
        }

        



        private string DetectNamaATM(string fileName)
        {
            if (fileName.Contains("HTC", StringComparison.OrdinalIgnoreCase)) return "HTC";
            if (fileName.Contains("WIN", StringComparison.OrdinalIgnoreCase)) return "WIN";
            if (fileName.Contains("HYS", StringComparison.OrdinalIgnoreCase)) return "HYS";
            return "UNKNOWN";
        }

        private string DetectJenisFile(string filePath)
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



        private string? ExtractTerminalId(string text)
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


        private string? ExtractAtmCode(string text)
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





        private string? ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!match.Success) return null;

            if (match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(match.Groups[2].Value))
                return match.Groups[2].Value.Trim();

            return match.Groups[1].Value.Trim();
        }


        private int? ExtractInt(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? int.Parse(match.Groups[1].Value) : null;
        }

        private decimal? ExtractDecimal(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            if (!match.Success) return null;

            var digitsOnly = Regex.Replace(match.Groups[1].Value, @"[^\d]", "");

            if (string.IsNullOrEmpty(digitsOnly)) return null;

            if (decimal.TryParse(digitsOnly, out var result))
                return result;

            return null;
        }



        private DateTime? ExtractDateTime(string text)
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
                return parsedDate; // ⬅️ langsung return, tidak ada AddHours
            }

            Console.WriteLine($"⚠️ Format tanggal tidak dikenal: {dateString}");
            return null;
        }




        private string? ExtractPesanError(string text)
        {
            var match = Regex.Match(text, @"PESAN\s*:\s*(.+?)(?:\n\s*\n|\Z)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var pesan = match.Groups[1].Value
                    .Replace("\r", "")
                    .Replace("\n", " ")
                    .Trim();
                return pesan;
            }
            return null;
        }



        private string? ExtractJenisTransaksi(string text)
        {
            if (text.Contains("PENARIKAN", StringComparison.OrdinalIgnoreCase)) return "Tarik Tunai";
            if (text.Contains("SETOR", StringComparison.OrdinalIgnoreCase)) return "Setor Tunai";
            if (text.Contains("LISTRIK", StringComparison.OrdinalIgnoreCase)) return "Pembelian Listrik";
            if (text.Contains("CEK SALDO", StringComparison.OrdinalIgnoreCase)) return "Cek Saldo";
            return "Lainnya";
        }
    }
}
