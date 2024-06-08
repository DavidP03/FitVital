using FitVital.Controllers;
using FitVital.DAL;
using FitVital.DAL.Entities;
using FitVital.DAL.Pojos;
using FitVital.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        //  Metodo para solicitar una cita
        public async Task<OperationResult> RequestAppointment(AppointmentRequest request)
        {
            // Se valida que el usuario existe
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new OperationResult {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Si tiene fecha final se asigna el dato, sino la fecha final será la inicial + 1h
            DateTime endTime = request.EndTime ?? request.StartTime.AddHours(1);

            // La fecha final no puede ser anterior a la inicial y no puede ser mayor a 2 horas de la inicial
            if (endTime <= request.StartTime || (endTime - request.StartTime).TotalHours > 2)
            {
                return new OperationResult { 
                    Success = false,
                    Message = "End time must be greater than start time and not exceed 2 hours."
                };
            }

            // Objeto a crear
            var newAppointment = new Appointment
            {
                StartTime = request.StartTime,
                EndTime = endTime,
                Description = request.Description,
                RequestedBy = user
            };

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();
            return new OperationResult {
                Success = true, Message = "OK",
                Object = newAppointment
            };
        }

        // Metodo para asignar una cita a un usuario
        public async Task<OperationResult> AssignAppointment(int appointmentId, int assignedToUserId)
        {
            // Se valida que existe la cita
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return new OperationResult { 
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            // Se valida que exista el usuario con rol de entrenador para poder asignarle la cita
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role) // Incluye la entidad de rol asociada
                .FirstOrDefaultAsync(u => u.UserId == assignedToUserId);
            if (user == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Se valida si el usuario asignado tiene rol de Trainer, ya que es requerido
            bool isTrainer = user.UserRoles.Any(ur => ur.Role.Name == "Trainer");

            if (!isTrainer)
            {
                return new OperationResult {
                    Success = false,
                    Message = "User is not a trainer" 
                };
            }

            // Si todo está bien se procede a hacer la asignación solicitada
            appointment.AssignedTo = user;
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true };
        }

        // Metodo para mostrar todos las citas
        public async Task<List<Appointment>> GetAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<Appointment> GetAppointmentById(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserId(int userId)
        {
            return await _context.Appointments
                .Where(a => a.RequestedById == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            // Verifica que la cita exista
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return false; // Cita no encontrada
            }

            // Procede con el borrado de la cita
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}