using FitVital.DAL;
using FitVital.DAL.Entities;
using FitVital.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DAL;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public AppointmentsController(DataBaseContext context)
        {
            _context = context;
        }

        // Método para solicitar una cita
        [HttpPost("request")]
        public async Task<IActionResult> RequestAppointment([FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Se valida que el usuario existe
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Si tiene fecha final se asigna el dato, sino la fecha final sera la inicial + 1h
            DateTime endTime = request.EndTime ?? request.StartTime.AddHours(1);

            // la fecha final no puede ser anterior a la inicial y no puede ser mayor a 2 horas de la inicial
            if (endTime <= request.StartTime || (endTime - request.StartTime).TotalHours > 2)
            {
                return BadRequest("End time must be greater than start time and not exceed 2 hours.");
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

            // Cargar solo las propiedades necesarias del nuevo compromiso
            var responseAppointment = new
            {
                newAppointment.AppointmentId,
                newAppointment.StartTime,
                newAppointment.EndTime,
                newAppointment.Description,
            };

            return CreatedAtAction(nameof(GetAppointmentById), new { id = newAppointment.AppointmentId }, responseAppointment);
        }

        // Método para Asignar una cita a un usuario
        [HttpPost("assign")]
        public async Task<IActionResult> AssignAppointment(int appointmentId, int assignedToUserId)
        {
            //Se valida que existe la cita
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            // Se valida que exista el usuario con rol de entrenador para poder asignarle la cita
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role) // Incluye la entidad de rol asociada
                .FirstOrDefaultAsync(u => u.UserId == assignedToUserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Se valida si el usuario asignado tenga rol de Trainer, ya que es requerido
            bool isTrainer = user.UserRoles.Any(ur => ur.Role.Name == "Trainer");

            if (!isTrainer)
            {
                return BadRequest("User is not a trainer");
            }

            //Si todo esta bien se procede a hacer la asignacion solicitada.
            appointment.AssignedTo = user;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Metodo para listar todas las citas
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return Ok(appointments);
        }

        // Metodo para mostrar cita basandose en su Id
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // Obtiene citas solicitadas por un usuario especifico
        [HttpGet("getRequestedByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByUserId(int userId)
        {
            // Valida que el usuario exista
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Se filtran las citas solicitadas por el usuario especificado
            var appointments = await _context.Appointments
                .Where(a => a.RequestedById == userId)
                .Select(a => new Appointment
                {
                    AppointmentId = a.AppointmentId,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Description = a.Description,
                    RequestedById = a.RequestedById,
                    AssignedToId = a.AssignedToId
                })
                .ToListAsync();

            // Si no se encuentran citas retorna NotFound
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound();
            }

            return Ok(appointments);
        }

        // Borrar Cita
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            // Verifica que la cita exista
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            // Procede con el borrado de la cita
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

    // Objeto auxiliar para realizar la solicitud de citas
    public class AppointmentRequest
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Description { get; set; } = "";
    }
}
