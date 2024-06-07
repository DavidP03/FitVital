using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebAPI.DAL;

namespace FitVital.Domain.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DataBaseContext _context;

        public AppointmentService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment newAppointment)
        {
            var user = await _context.Users.FindAsync(newAppointment.RequestedBy);

            if (user == null)
            {
                throw new System.Exception("Invalid user.");
            }

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();
            return newAppointment;
        }

        public async Task<(Appointment, string)> UpdateAppointmentAsync(int id, Appointment updatedAppointment)
        {
            try
            {
                var existingAppointment = await _context.Appointments.FindAsync(id);
                if (existingAppointment == null)
                {
                    return (null, "Appointment not found");
                }

                _context.Appointments.Update(updatedAppointment);
                await _context.SaveChangesAsync();

                return (existingAppointment, "Appointment updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, $"Failed to update appointment: {ex.Message}");
            }
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return await _context.Appointments.Where(a => a.RequestedBy == user).ToListAsync();
        }
    }
}
