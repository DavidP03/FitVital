using FitVital.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DAL;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public RolesController(DataBaseContext context)
        {
            _context = context;
        }

        // Metodo para crear nuevo Rol
        [HttpPost("create")]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Objeto a crear
            var newRole = new Role
            {
                Name = request.name
            };

            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleById), new { id = newRole.RoleId }, newRole);
        }

        // Metodo para mostrar todos los roles
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // metodo para mostrar rol basado en el Id
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // Metodo para borrar rol basado en el Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            // Valida que el rol especificado existe
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            // Valida si el rol especificado está en uso, es decir si un usuario tiene asignado este rol
            var isRoleInUse = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id);
            if (isRoleInUse)
            {
                return BadRequest("Role is in use and cannot be deleted.");
            }

            // Se elimina rol
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Objeto auxiliar para realizar la solicitud de creacion de roles
    public class RoleRequest
    {
        public string name { get; set; }
    }
}
