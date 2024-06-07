using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FitVital.DAL.Entities
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; } // Fecha y hora de inicio de la cita
        public DateTime EndTime { get; set; } // Fecha y hora de finalización de la cita, si no se especifica sera 1hora despues de la fecha de inicio

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int RequestedById { get; set; }
        public User RequestedBy { get; set; }

        public int? AssignedToId { get; set; } 
        public User AssignedTo { get; set; }
    }
}
