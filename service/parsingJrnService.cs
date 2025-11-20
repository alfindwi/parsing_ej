using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using parsing_jrn_ej.dto.transaksi;
using parsing_Jrn_Ej.Data;
using parsing_Jrn_Ej.dto;
using parsing_Jrn_Ej.Models;
using parsing_Jrn_Ej.response;
using parsing_Jrn_Ej.Helpers;

namespace parsing_jrn_Ej.Services
{
    public class JrnParserService
    {
        private readonly AppDbContext _context;
        private readonly ParsingHelper _parser = new ParsingHelper();
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
                    var terminalId = _parser.ExtractTerminalId(text) ?? _parser.ExtractAtmCode(text);

                    var transaksi = new AtmTransaksi
                    {
                        JenisFile = _parser.DetectJenisFile(Path.Combine(_basePath, fileName)),
                        NamaAtm = _parser.DetectNamaATM(fileName),
                        NoTransaksi = _parser.ExtractInt(text, @"(\d+)\s+\d{2}/\d{2}/\d{4}"),
                        Waktu = _parser.ExtractDateTime(text),
                        NoKartu = _parser.ExtractValue(text, @"(?:Card Number|NO KARTU)\s*[:\]]\s*([0-9X*]+)"),
                        JenisTransaksi = _parser.ExtractJenisTransaksi(text),
                        TerminalId = terminalId,
                        AtmId = _parser.ExtractValue(text, @"ATM ID\s*:([0-9]+)"),
                        Lokasi = _parser.ExtractValue(text, @"LOKASI\s*:?(.+)"),
                        OpCode = _parser.ExtractValue(text, @"(?:OP Code|TRANSACTION REQUEST).*\[(.*?)\]"),
                        Jumlah = _parser.ExtractDecimal(text, @"JUMLAH\s*[:=]\s*RP[.\s]*(\d+[.,]?\d*)"),
                        Saldo = _parser.ExtractDecimal(text, @"SALDO\s*[:=]\s*RP[.\s]*(\d+[.,]?\d*)"),
                        NoRef = _parser.ExtractValue(text, @"NO\s*REF{1,2}\.?\s*[:=]\s*([0-9]+)"),
                        NoRekening = _parser.ExtractValue(text, @"REKENING[:=]\s*([0-9]+)"),
                        PesanError = _parser.ExtractPesanError(text),
                        Struk = text,
                        FunctionIdentifier = _parser.ExtractValue(text, @"FUNCTION IDENTIFIER\s*[:=]\s*(.*)"),
                        TransSeqNumber = _parser.ExtractValue(text, @"TRANSACTION SEQUENCE NUMBER\s*[:=]\s*(.*)"),
                        Tsi = _parser.ExtractValue(text, @"TSI\s*[:=]\s*(.*)"),
                        Tvr = _parser.ExtractValue(text, @"TVR\s*[:=]\s*([A-Fa-f0-9]{6,})")


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

        public async Task<PaginatedResponse<AtmTransaksiDto>> getAllTransaksi(int page = 1)
        {
            int pageSize = 100;
            int offset = (page - 1) * pageSize;

            string rawSql = @"
        SELECT 
            id,
            jenis_file AS JenisFile,
            no_transaksi AS NoTransaksi,
            waktu AS Waktu,
            no_kartu AS NoKartu,
            jenis_transaksi AS JenisTransaksi,
            terminal_id AS TerminalId,
            nama_atm AS NamaAtm,
            atm_id AS AtmId,
            lokasi AS Lokasi,
            op_code AS OpCode,
            jumlah AS Jumlah,
            saldo AS Saldo,
            struk AS Struk,
            no_ref AS NoRef,
            no_rekening AS NoRekening,
            tvr AS Tvr,
            tsi AS Tsi,
            function_identifier AS FunctionIdentifier,
            trans_seq_number AS TransSeqNumber,
            pesan_error AS PesanError,
            CreatedAt,
            COUNT(*) OVER() AS TotalCount
        FROM atm_transaksi
        ORDER BY id
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;
    ";

            var result = await _context.Set<AtmTransaksiWithCountDto>()
                .FromSqlRaw(rawSql,
                    new SqlParameter("@offset", offset),
                    new SqlParameter("@pageSize", pageSize)
                )
                .AsNoTracking()
                .ToListAsync();

            int totalData = result.FirstOrDefault()?.TotalCount ?? 0;
            int totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

            var list = result.Select(x => new AtmTransaksiDto
            {
                Id = x.Id,
                JenisFile = x.JenisFile,
                NoTransaksi = x.NoTransaksi,
                Waktu = x.Waktu,
                NoKartu = x.NoKartu,
                JenisTransaksi = x.JenisTransaksi,
                TerminalId = x.TerminalId,
                NamaAtm = x.NamaAtm,
                AtmId = x.AtmId,
                Lokasi = x.Lokasi,
                OpCode = x.OpCode,
                Jumlah = x.Jumlah,
                Saldo = x.Saldo,
                Struk = x.Struk,
                NoRef = x.NoRef,
                NoRekening = x.NoRekening,
                Tvr = x.Tvr,
                Tsi = x.Tsi,
                FunctionIdentifier = x.FunctionIdentifier,
                TransSeqNumber = x.TransSeqNumber,
                PesanError = x.PesanError,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PaginatedResponse<AtmTransaksiDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalData = totalData,
                TotalPages = totalPages,
                Data = list
            };
        }


        public async Task<List<object>> getPesanError()
        {
            var rawData = await _context.Set<PesanErrorDto>()
                .FromSqlRaw(@"
            SELECT 
                pesan_error AS PesanError,
                UPPER(SUBSTRING(nama_atm, 1, 3)) AS Merk,
                COUNT(*) AS Total
            FROM atm_transaksi
            WHERE pesan_error IS NOT NULL 
              AND pesan_error != ''
            GROUP BY pesan_error, UPPER(SUBSTRING(nama_atm, 1, 3))
            ORDER BY pesan_error
        ")
                .ToListAsync();

            var cleaned = rawData
                .Where(x => x.PesanError != null && x.Merk != null)
                .Select(x => new
                {
                    PesanError = x.PesanError!,
                    Merk = x.Merk!,
                    Total = x.Total
                })
                .ToList();

            var allMerk = cleaned
                .Select(x => x.Merk)
                .Distinct()
                .ToList();

            var result = cleaned
                .GroupBy(x => x.PesanError)
                .Select(g => new
                {
                    detail = g.Key,
                    namaAtm = allMerk.ToDictionary(
                        merk => merk,
                        merk => g.Where(x => x.Merk == merk).Sum(x => x.Total)
                    )
                })
                .ToList<object>();

            return result;
        }


    }
}
