using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parsing_jrn_ej.dto.transaksi
{
    public class AtmTransaksiWithCountDto
    {
        public int Id { get; set; }
        public string? JenisFile { get; set; }
        public int? NoTransaksi { get; set; }
        public DateTime? Waktu { get; set; }
        public string? NoKartu { get; set; }
        public string? JenisTransaksi { get; set; }
        public string? TerminalId { get; set; }
        public string? NamaAtm { get; set; }
        public string? AtmId { get; set; }
        public string? Lokasi { get; set; }
        public string? OpCode { get; set; }
        public decimal? Jumlah { get; set; }
        public decimal? Saldo { get; set; }
        public string? Struk { get; set; }
        public string? NoRef { get; set; }
        public string? NoRekening { get; set; }
        public string? Tvr { get; set; }
        public string? Tsi { get; set; }
        public string? FunctionIdentifier { get; set; }
        public string? TransSeqNumber { get; set; }
        public string? PesanError { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TotalCount { get; set; }
    }

}