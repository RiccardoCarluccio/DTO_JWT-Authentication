using Esercizio_DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace Esercizio_DTO.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Prodotto> Prodotti { get; set; }
    }
}
