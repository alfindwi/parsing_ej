using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjParser.Models
{
    [Table("atm_session")]
    public class AtmSession
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string? TerminalId { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [MaxLength(10)]
        public string? MachineType { get; set; } // Contoh: HYS, HTC, WIN

        public string? RawLog { get; set; }

        // Relasi ke transaksi
        public ICollection<TransaksiAtm>? Transaksis { get; set; }
    }
}
