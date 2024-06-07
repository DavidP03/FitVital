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

        // Este metodo se usa para aplicar configuraciones especiales como el usuario y la Base de datos
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=FitVitalDB;User ID=sa;Password=saPass;");
        }


        //Este método que es propio de EF CORE me sirve para configurar unos índices de cada campo de una tabla en BD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la relación muchos a muchos entre User y Role
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configurar relaciones para Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.RequestedBy)
                .WithMany(u => u.AppointmentsRequested)
                .HasForeignKey(a => a.RequestedById)
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminación en cascada

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AssignedTo)
                .WithMany(u => u.AppointmentsAssigned)
                .HasForeignKey(a => a.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminación en cascada

            // Datos semilla para roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Client" },
                new Role { RoleId = 2, Name = "Admin" },
                new Role { RoleId = 3, Name = "Trainer" }
            );
        }

        #region DbSets

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Role> Roles { get; set; }

        #endregion
    }
}
