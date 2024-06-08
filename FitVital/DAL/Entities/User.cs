using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FitVital.DAL.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "date")] // Esto asegura que solo se guarde la fecha en la base de datos
        public DateTime BirthDate { get; set; }

        [Required]
        [RegularExpression("^[MFO]$", ErrorMessage = "Gender must be 'M', 'F', or 'O'")]
        [MaxLength(1)]
        public string Gender { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); // Relación con UserRoles
        public virtual ICollection<Appointment> AppointmentsRequested { get; set; } = new List<Appointment>(); // Relación con citas solicitadas
        public virtual ICollection<Appointment> AppointmentsAssigned { get; set; } = new List<Appointment>(); // Relación con citas asignadas como entrenador
    }
}
