using Microsoft.EntityFrameworkCore;
using parsing_Jrn_Ej.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace parsing_Jrn_Ej.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AtmTransaksi> AtmTransaksi { get; set; }

        
    }
}
