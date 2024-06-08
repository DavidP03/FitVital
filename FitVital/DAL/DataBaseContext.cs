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
            base.OnConfiguring(optionsBuilder);
        }

        //Este método que es propio de EF CORE me sirve para configurar unos índices de cada campo de una tabla en BD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la propiedad Username como única
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

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

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AssignedTo)
                .WithMany(u => u.AssignedAppointments) // O con WithOne si la relación es uno a uno
                .HasForeignKey(a => a.AssignedToId);

            // Datos semilla para roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Client" },
                new Role { RoleId = 2, Name = "Admin" },
                new Role { RoleId = 3, Name = "Trainer" }
            );

            // Agregar usuarios al inicializar la base de datos
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "paquito89",
                    Password = "pass",
                    Email = "paquito89@example.com",
                    FirstName = "Paco",
                    LastName = "Tilla",
                    PhoneNumber = "1234567",
                    BirthDate = new DateTime(1989, 1, 1),
                    Gender = "M"
                },
                new User
                {
                    UserId = 2,
                    Username = "carlos45hd",
                    Password = "pass",
                    Email = "carlos45hd@example.com",
                    FirstName = "Carlos",
                    LastName = "Hurtado",
                    PhoneNumber = "7654321",
                    BirthDate = new DateTime(1995, 6, 15),
                    Gender = "M"
                }
            );

            // Asignar roles a los usuarios al inicializar la base de datos
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // Usuario 1 (cliente)
                new UserRole { UserId = 2, RoleId = 3 }  // Usuario 2 (entrenador)
            );
        }

        #region DbSets

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion
    }
}
