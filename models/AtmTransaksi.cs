using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace parsing_Jrn_Ej.Models
{
    [Table("atm_transaksi")]
    public class AtmTransaksi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("jenis_file")]
        public string? JenisFile { get; set; }

        [Column("no_transaksi")]
        public int? NoTransaksi { get; set; }

        [Column("waktu", TypeName = "datetime")]
        public DateTime? Waktu { get; set; }

        [Column("no_kartu")]
        public string? NoKartu { get; set; }

        [Column("jenis_transaksi")]
        public string? JenisTransaksi { get; set; }

        [Column("terminal_id")]
        public string? TerminalId { get; set; }

        [Column("nama_atm")]
        public string? NamaAtm { get; set; }

        [Column("atm_id")]
        public string? AtmId { get; set; }

        [Column("lokasi")]
        public string? Lokasi { get; set; }

        [Column("op_code")]
        public string? OpCode { get; set; }

        [Column("jumlah", TypeName = "numeric")]
        public decimal? Jumlah { get; set; }

        [Column("saldo", TypeName = "numeric")]
        public decimal? Saldo { get; set; }

        [Column("struk")]
        public string? Struk { get; set; }

        [Column("no_ref")]
        public string? NoRef { get; set; }

        [Column("no_rekening")]
        public string? NoRekening { get; set; }

        [Column("pesan_error")]
        public string? PesanError { get; set; }

        [Column("dibuat_pada", TypeName = "datetime")]
        public DateTime DibuatPada { get; set; } = DateTime.UtcNow;
    }

}
