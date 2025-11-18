using Microsoft.EntityFrameworkCore;
using parsing_Jrn_Ej.Models;
using parsing_Jrn_Ej.DTO;

namespace parsing_Jrn_Ej.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AtmTransaksi> AtmTransaksi { get; set; }

        public DbSet<PesanErrorRaw> PesanErrorRaw { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PesanErrorRaw>().HasNoKey();
        }

    }
}
