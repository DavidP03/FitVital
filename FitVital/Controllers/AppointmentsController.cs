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
        private AppointmentService _appointmentService;
        private UserService _userService;

        public AppointmentsController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment(Appointment newAppointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.CreateAppointmentAsync(newAppointment);
            if (result != null)
                return CreatedAtAction(nameof(GetAppointmentById), new { id = result.AppointmentId }, result);
            else
                return BadRequest();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignAppointment(int appointmentId, int assignedToUserId)
        {
            //Se valida que existe la cita
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            // Se valida que exista el usuario con rol de entrenador
            var user = await _context.Users
                .Include(u => u.UserRoles) // Asegúrate de cargar los roles del usuario
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
            var result = await _appointmentService.UpdateAppointmentAsync(appointmentId, appointment);
            if (result.Item1 != null)
            {
                return Ok();
            } else {
                return BadRequest();
            }
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
}
