using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WebAPI.DAL;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public UsersController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRoles = newUser.UserRoles;

            foreach (var userRole in userRoles) {
                var role = userRole.Role;
                var result = await _context.Roles.FindAsync(role.RoleId);
                if (result == null)
                {
                    return BadRequest("Role not found: " + role.Name);
                }
            }           

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, newUser);
        }

        // Asignación de roles al usuario
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRoles(int userId, [FromBody] List<int> roleIds)
        {
            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            foreach (var roleId in roleIds)
            {
                if (!user.UserRoles.Any(ur => ur.RoleId == roleId))
                {
                    user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // Obtener todos los usuarios
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // Obtener usuarios activos
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }

        // Obtener usuarios activos según su rol
        [HttpGet("active/role/{roleName}")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsersByRole(string roleName)
        {
            return await _context.Users
                                 .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.Role.Name == roleName))
                                 .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
