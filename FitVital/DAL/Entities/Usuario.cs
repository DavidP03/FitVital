using System.ComponentModel.DataAnnotations;

namespace FitVital.DAL.Entities
{
    public class Usuario
    {
        [Key] //PK
        public Guid UsuarioId { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El campo {0} es un campo con formato email")]
        public string Correo { get; set; }

        public int Edad { get; set; }

        [MaxLength(9, ErrorMessage = "El campo {0} admite como maximo {1} caracteres")]
        public string Genero { get; set; }

        //// Relación uno a muchos con Agenda
        //public List<Agenda>? Agenda { get; set; } = new List<Agenda>();
    }
}
