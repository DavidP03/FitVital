using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // Método para solicitar una cita
        [HttpPost("request")]
        public async Task<IActionResult> RequestAppointment([FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _appointmentService.RequestAppointment(request);
            if (response == null)
            {
                return BadRequest("Invalid appointment request.");
            }

            var responseObject = (Appointment)response.Object;

            var responseAppointment = new
            {
                responseObject.AppointmentId,
                responseObject.StartTime,
                responseObject.EndTime,
                responseObject.Description,
            };

            return CreatedAtAction(nameof(GetAppointmentById), new { id = responseObject.AppointmentId }, responseAppointment);
        }

        // Método para Asignar una cita a un usuario con rol trainer
        [HttpPost("assign")]
        public async Task<IActionResult> AssignAppointment(int appointmentId, int assignedToUserId)
        {
            var result = await _appointmentService.AssignAppointment(appointmentId, assignedToUserId);
            if (result == null)
            {
                return BadRequest("Assignment failed.");
            }

            if(result.Success)
            {
                return Ok();
            } else
            {
                return BadRequest(result.Message);
            }
        }

        // Metodo para listar todas las citas
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _appointmentService.GetAppointments();
            return Ok(appointments);
        }

        // Metodo para mostrar cita basandose en su Id
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);

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
            var appointments = await _appointmentService.GetAppointmentsByUserId(userId);

            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found for the specified user.");
            }

            return Ok(appointments);
        }

        // Borrar Cita
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _appointmentService.DeleteAppointment(id);
            if (result == null)
            {
                return NotFound("Appointment not found.");
            }

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