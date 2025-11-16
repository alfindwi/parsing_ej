using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjParser.Models
{
    [Table("atm_transaksi")]
    public class TransaksiAtm
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Session")]
        public int? SessionId { get; set; }
        public AtmSession? Session { get; set; }

        [MaxLength(20)]
        public string? TransSeqNumber { get; set; }

        [MaxLength(20)]
        public string? OpCode { get; set; }

        [MaxLength(50)]
        public string? CardNumber { get; set; }

        [MaxLength(20)]
        public string? Amount { get; set; }

        [MaxLength(20)]
        public string? TerminalId { get; set; }

        [MaxLength(10)]
        public string? MachineType { get; set; }

        [MaxLength(50)]
        public string? Tvr { get; set; }

        [MaxLength(50)]
        public string? Tsi { get; set; }

        public string? JprContents { get; set; }

        [MaxLength(100)]
        public string? Lokasi { get; set; }

        public DateTime? TransactionTime { get; set; }

        [MaxLength(100)]
        public string? FileName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
