using Microsoft.EntityFrameworkCore;
using parsing_Jrn_Ej.Models;
using parsing_Jrn_Ej.dto;
using parsing_jrn_ej.dto.transaksi;

namespace parsing_Jrn_Ej.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AtmTransaksi> AtmTransaksi { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AtmTransaksiDto>().HasNoKey();
            modelBuilder.Entity<PesanErrorDto>().HasNoKey();
            modelBuilder.Entity<AtmTransaksiWithCountDto>().HasNoKey();

        }

    }
}
