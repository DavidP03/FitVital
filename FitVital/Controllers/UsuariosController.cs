using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet, ActionName("Get")]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosAsync()
        {
            var usuarios = await _usuarioService.GetUsuariosAsync();

            if (usuarios == null || !usuarios.Any()) return NotFound();

            return Ok(usuarios);
        }

        [HttpGet, ActionName("Get")]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByIdAsync(Guid id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);

            if (usuario == null) return NotFound(); //NotFound = Status Code 404

            return Ok(usuario); //Ok = Status Code 200
        }

        [HttpPost, ActionName("Create")]
        [Route("Create")]
        public async Task<ActionResult<Usuario>> Post(Usuario usuario)
        {
            try
            {
                var newUsuario = await _usuarioService.PostUsuarioAsync(usuario);
                if (newUsuario == null) return NotFound();
                return Ok(newUsuario);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate"))
                    return Conflict(String.Format("{0} ya existe", usuario.Nombre));

                return Conflict("Error a la hora de insertar");
            }
        }

        [HttpDelete, ActionName("Delete")]
        [Route("Delete")]
        public async Task<ActionResult<Usuario>> DeleteUsuarioAsync(Guid id)
        {
            if (id == null) return BadRequest();

            var deletedUsuario = await _usuarioService.DeleteUsuarioAsync(id);

            if (deletedUsuario == null) return NotFound();

            return Ok("Borrado satisfactoriamente");

        }
    }
}
