using FitVital.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.DAL
{
    public class DataBaseContext : DbContext
    {
        //Así me conecto a la BD por medio de este constructor
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {

        }

        //Este método que es propio de EF CORE me sirve para configurar unos índices de cada campo de una tabla en BD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Entrenador>().HasIndex(c => c.EntrenadorId).IsUnique(); // Haciendo un índice compuesto
            modelBuilder.Entity<Usuario>().HasIndex(c => c.UsuarioId).IsUnique(); // Haciendo un índice compuesto
            modelBuilder.Entity<Agenda>().HasIndex(c => c.CitaId).IsUnique();
            modelBuilder.Entity<Ejercicio>().HasIndex(c => c.EjercicioId).IsUnique();
        }

        #region DbSets

        public DbSet<Entrenador> Entrenadores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }

        #endregion
    }
}
