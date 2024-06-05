using FitVital.DAL.Entities;
using fitVital_API.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntrenadorController : Controller
    {

        private readonly IEntrenadorService _entrenadorService;

        public EntrenadorController(IEntrenadorService entrenadorService)
        {
            _entrenadorService = entrenadorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEntrenadores()
        {
            try
            {
                var entrenadores = await _entrenadorService.GetEntrenadores();
                return Ok(entrenadores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntrenadorById(Guid id)
        {
            try
            {
                var entrenador = await _entrenadorService.GetEntrenadorById(id);
                return Ok(entrenador);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostEntrenador(Entrenador entrenador)
        {
            try
            {
                var entrenadorCreado = await _entrenadorService.PostEntrenador(entrenador);
                return Ok(entrenadorCreado);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Message);
                throw new Exception("No recibe entrenador");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DesactivarEntrenador(Guid id)
        {
            try
            {
                var entrenadorDesactivado = await _entrenadorService.DesactivarEntrenador(id);
                return Ok("Entrenador desactivado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
