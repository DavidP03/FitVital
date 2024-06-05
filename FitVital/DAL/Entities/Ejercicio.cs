namespace FitVital.DAL.Entities
{
    public class Ejercicio
    {
        public Guid EjercicioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int DuracionMinutos { get; set; }
    }
}
