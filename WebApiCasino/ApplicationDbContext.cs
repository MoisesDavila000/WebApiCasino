using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ParticipantesRifasCartas>()
                .HasKey(r => new { r.IdParticipante, r.IdRifa, r.IdCarta });
        }

        public DbSet<Participantes> Participantes { get; set; }

        public DbSet<Rifas> Rifas { get; set; }

        public DbSet<CartasLoteMex> Cartas { get; set; }

        public DbSet<Premios> Premios { get; set; }

        public DbSet<ParticipantesRifasCartas> ParticipantesRifasCartas { get; set; }

    }
}
