namespace FitVital.DAL.Pojos
{
    public class AppointmentResponse
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public string TrainerName { get; set; }
    }
}
