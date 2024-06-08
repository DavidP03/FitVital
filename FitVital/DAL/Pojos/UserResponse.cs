using FitVital.Controllers;

namespace FitVital.DAL.Pojos
{
    // Objeto auxiliar para formato a las respuestas del API
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<int, string> UserRoles { get; set; }
        public List<AppointmentResponse> AssignedAppointments { get; set; } // Citas asignadas
        public List<AppointmentResponse> RequestedAppointments { get; set; } // Citas solicitadas
    }
}
