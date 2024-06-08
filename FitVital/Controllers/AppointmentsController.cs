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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return Ok(appointments);
        }

        // Método para solicitar una cita
        [HttpPost("request")]
        public async Task<IActionResult> RequestAppointment([FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            DateTime endTime = request.EndTime ?? request.StartTime.AddHours(1);

            if (endTime <= request.StartTime || (endTime - request.StartTime).TotalHours > 2)
            {
                return BadRequest("End time must be greater than start time and not exceed 2 hours.");
            }

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
                // Agregar otras propiedades según sea necesario
            };

            return CreatedAtAction(nameof(GetAppointmentById), new { id = newAppointment.AppointmentId }, responseAppointment);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignAppointment(int appointmentId, int assignedToUserId)
        {
            //Se valida que existe la cita
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            // Se valida que exista el usuario con rol de entrenador
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role) // Incluye la entidad de rol asociada
                    .FirstOrDefaultAsync(u => u.UserId == assignedToUserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByUserId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var appointments = await _context.Appointments.Where(a => a.RequestedBy == user).ToListAsync();

            if (appointments == null || appointments.Count == 0)
            {
                return NotFound();
            }

            return appointments;
        }
    }

    public class AppointmentRequest
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Description { get; set; } = "";
    }
}
