using System.ComponentModel.DataAnnotations;

namespace FitVital.DAL.Entities
{
    public class Entrenador
    {
        [Key] //PK
        public Guid EntrenadorId { get; set; }

        public string Nombre { get; set; }

        public string Especialidad { get; set; }

        public Boolean Activo { get; set; }

        // Relación uno a muchos con Agenda
        public List<Agenda> Agenda { get; set; } = new List<Agenda>();
    }
}
