using System.ComponentModel.DataAnnotations;

namespace FitVital.DAL.Entities
{
    public class Agenda
    {
        [Key]
        public Guid CitaId { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public Guid EntrenadorId { get; set; }
        public Entrenador Entrenador { get; set; }

        public DateTime FechaHora { get; set; }
    }
}
