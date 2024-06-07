using FitVital.DAL.Entities;

namespace FitVital.Domain.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(Appointment newAppointment);
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(int userId);
    }
}
