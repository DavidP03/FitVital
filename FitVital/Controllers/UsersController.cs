using FitVital.DAL.Entities;
using FitVital.DAL.Pojos;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitVital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Metodo para crear nuevo Usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] userRequest request)
        {
            var result = await _userService.RegisterAsync(request);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = ((User)result.Object).UserId }, result.Object);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT: api/Users/update/{userId}
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] userRequest request)
        {
            var result = await _userService.UpdateUserAsync(userId, request);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // GET: api/Users/getAll
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (result != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        // GET: api/Users/getActiveUsers
        [HttpGet("getActiveUsers")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _userService.GetActiveUsersAsync();
            if (result != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        // GET: api/Users/getById/{id}
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            UserResponse response = await _userService.GetUserByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        // GET: api/Users/getByRoleName/{roleName}
        [HttpGet("getByRoleName/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var result = (OperationResult) await _userService.GetUsersByRoleAsync(roleName);

            if (result.Success)
            {
                return Ok((UserResponse)result.Object);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT: api/Users/disable/{userId}
        [HttpPut("disable/{userId}")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            var result = (OperationResult) await _userService.DisableUserAsync(userId);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT: api/Users/enable/{userId}
        [HttpPut("enable/{userId}")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            var result = (OperationResult) await _userService.EnableUserAsync(userId);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT: api/Users/{userId}/assignRole
        [HttpPut("{userId}/assignRole")]
        public async Task<IActionResult> AssignRoles(int userId, [FromBody] List<int> roleIds)
        {
            var result = (OperationResult) await _userService.AssignRolesAsync(userId, roleIds);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT: api/Users/{userId}/unassignRole/{roleId}
        [HttpPut("{userId}/unassignRole/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var result = (OperationResult) await _userService.RemoveRoleFromUserAsync(userId, roleId);

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}




/*

using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using WebAPI.DAL;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System;
using FitVital.DAL.Pojos;

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
        #region POST
        // Metodo para crear nuevo Usuario
        [HttpPost("register")]
        public async Task<OperationResult> RegisterAsync(userRequest request)
        {
            try
            {
                if (request.BirthDate == null)
                {
                    return new OperationResult { Success = false, Message = "The birthdate field is required.", Object = null };
                }

                var gender = request.Gender;
                if (gender != "M" && gender != "F" && gender != "O")
                {
                    return new OperationResult { Success = false, Message = "Gender must be 'M', 'F', or 'O'.", Object = null };
                }

                var newUser = new User
                {
                    Username = request.Username,
                    Password = request.Password,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    BirthDate = (DateTime)request.BirthDate,
                    Gender = request.Gender
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "User registered successfully.", Object = newUser };
            }
            catch (Exception ex)
            {
                // Log the exception
                return new OperationResult { Success = false, Message = ex.Message, Object = null };
            }
        }
        #endregion

        #region GET
        // Obtener todos los usuarios
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
        {
            var users = await _context.Users
               .Include(u => u.UserRoles)
               .ThenInclude(ur => ur.Role)
               .Include(u => u.AssignedAppointments)
               .Include(u => u.RequestedAppointments)
               .ToListAsync();

            return users.Select(MapUserToResponse).ToList();
        }

        // Obtener usuarios activos
        [HttpGet("getActiveUsers")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetActiveUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.AssignedAppointments)
                .Include(u => u.RequestedAppointments)
                .ToListAsync();

            return users.Select(MapUserToResponse).ToList();
        }

        // Metodo para consultar un usuario especificando su Id
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.AssignedAppointments)
                .Include(u => u.RequestedAppointments)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return MapUserToResponse(user);
        }

        // Obtener usuarios según su rol
        [HttpGet("getByRoleName/{roleName}")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsersByRole(string roleName)
        {
            var users = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.AssignedAppointments)
                .Include(u => u.RequestedAppointments)
                .ToListAsync();

            return users.Select(MapUserToResponse).ToList();
        }
        #endregion

        #region PUT
        // Método para actualizar la información de un usuario
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] userRequest request)
        {
            // Valida que el usuario exista
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verifica si el género es M, F o O
            var gender = request.Gender;
            if (gender != "M" && gender != "F" && gender != "O")
            {
                return BadRequest("Gender must be 'M', 'F', or 'O'.");
            }

            // Actualizar los campos del usuario con los datos proporcionados con excepcion de usuario y password
            user.Username = user.Username;
            user.Password = user.Password;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.BirthDate = request.BirthDate.HasValue ? request.BirthDate.Value : user.BirthDate;
            user.Gender = request.Gender ?? user.Gender;

            _context.Users.Update(user);
            return await SaveChangesAsyncWithExceptionHandling();
        }

        // Método para deshabilitar un usuario
        [HttpPut("disable/{userId}")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Obtener la fecha y hora actuales
            var now = DateTime.UtcNow;

            // Buscar y eliminar las citas futuras del usuario
            var futureAppointments = await _context.Appointments
                .Where(a => a.RequestedById == userId && a.StartTime > now)
                .ToListAsync();

            if (futureAppointments.Any())
            {
                _context.Appointments.RemoveRange(futureAppointments);
            }

            // Deshabilitar al usuario
            user.IsActive = false;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok("User disabled and future appointments deleted successfully.");
        }

        // Método para habilitar un usuario
        [HttpPut("enable/{userId}")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            // Valida que el usuario exista
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Habilitar al usuario
            user.IsActive = true;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok("User enabled successfully.");
        }

        // Metodo para asignar roles a un usuario especifico
        [HttpPut("{userId}/assignRole")]
        public async Task<IActionResult> AssignRoles(int userId, [FromBody] List<int> roleIds)
        {
            // Valida si el usuario existe
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Para cada rol especificado en el request, si existe, lo asigna al usuario
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

        // Metodo para quitar un rol especificado a un usuario
        [HttpPut("{userId}/unassignRole/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verificar si el rol existe
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            // Buscar la relación entre el usuario y el rol
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
            {
                return BadRequest("The user does not have this role assigned");
            }

            // Eliminar la relación
            user.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Private Methods
        // Metodo usado por register y update para manejar errores por violacion de indice unico
        private async Task<IActionResult> SaveChangesAsyncWithExceptionHandling()
        {
            try
            {
                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                return Ok("Operation successful.");
            }
            catch (DbUpdateException ex)
            {
                // Verifica si la excepción se debe a una violación de índice único
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    var errorMessage = sqlEx.Message;
                    var index = errorMessage.IndexOf("'");
                    var columnName = errorMessage.Substring(index + 1, errorMessage.IndexOf("'", index + 1) - index - 1);

                    // Forma un mensaje indicando el campo que viola el índice único
                    var errorMessageWithColumn = $"Duplicate entry for unique index. The value of '{columnName}' is already in use.";

                    return BadRequest(errorMessageWithColumn);
                }
                else
                {
                    // Si es una excepcion diferente la lanza
                    throw;
                }
            }
        }

        // Método privado para mapear un usuario a un objeto UserResponse
        private UserResponse MapUserToResponse(User user)
        {
            var assignedAppointments = _context.Appointments
                .Where(a => a.AssignedToId == user.UserId)
                .Select(a => new AppointmentResponse
                {
                    StartTime = a.StartTime,
                    EndTime = (DateTime)a.EndTime,
                    Description = a.Description,
                    TrainerName = $"{user.FirstName} {user.LastName}"
                })
                .ToList();

            var requestedAppointments = _context.Appointments
                .Where(a => a.RequestedById == user.UserId)
                .Select(a => new AppointmentResponse
                {
                    StartTime = a.StartTime,
                    EndTime = (DateTime)a.EndTime,
                    Description = a.Description,
                    TrainerName = $"{user.FirstName} {user.LastName}"
                })
                .ToList();

            return new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                IsActive = user.IsActive,
                UserRoles = user.UserRoles.ToDictionary(
                    ur => ur.Role.RoleId,
                    ur => ur.Role.Name
                ),
                AssignedAppointments = assignedAppointments,
                RequestedAppointments = requestedAppointments
            };
        }
        #endregion
    }
}
*/