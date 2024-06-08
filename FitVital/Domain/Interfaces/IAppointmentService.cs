using FitVital.Controllers;
using FitVital.DAL.Entities;
using FitVital.DAL.Pojos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitVital.Domain.Interfaces
{
    public interface IAppointmentService
    {
        Task<OperationResult> RequestAppointment(AppointmentRequest request);
        Task<OperationResult> AssignAppointment(int appointmentId, int assignedToUserId);
        Task<List<Appointment>> GetAppointments();
        Task<Appointment> GetAppointmentById(int id);
        Task<List<Appointment>> GetAppointmentsByUserId(int userId);
        Task<bool> DeleteAppointment(int id);
    }
}