using FitVital.Controllers;
using FitVital.DAL.Entities;
using FitVital.DAL.Pojos;
using FitVital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.DAL;

namespace FitVital.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly DataBaseContext _context;

        public UserService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> RegisterAsync(userRequest request)
        {
         
            // Verifica si el género es M, F o O
            var gender = request.Gender;
            if (gender != "M" && gender != "F" && gender != "O")
            {
                return new OperationResult { Success = false, Message = "Gender must be 'M', 'F', or 'O'.", Object = null };
            }

            if (request.BirthDate == null)
            {
                return new OperationResult { Success = false, Message = "The birthdate field is required.", Object = null };
            }

            // Objeto a crear
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

            var result = await SaveChangesAsyncWithExceptionHandling(); //Metodo personalizado para manejar excepcion de violacion de indice unico

            if (result is BadRequestObjectResult) // Si la respuesta es un BadRequest, retorna el resultado
            {
                return new OperationResult { Success = false, Message = "Registration failed.", Object = null };
            }
            else
            {
                return new OperationResult { Success = true, Message = "User registered successfully.", Object = newUser };
            }
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.AssignedAppointments)
                .Include(u => u.RequestedAppointments)
                .ToListAsync();

            return users.Select(MapUserToResponse).ToList();
        }

        public async Task<IEnumerable<UserResponse>> GetActiveUsersAsync()
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

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.AssignedAppointments)
                .Include(u => u.RequestedAppointments)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return null;
            }

            return MapUserToResponse(user);
        }

        public async Task<IEnumerable<UserResponse>> GetUsersByRoleAsync(string roleName)
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

        public async Task<OperationResult> UpdateUserAsync(int userId, userRequest request)
        {
            // Valida que el usuario exista
            User? user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found.", Object = null };
            }

            // Verifica si el género es M, F o O
            var gender = request.Gender;
            if (gender != "M" && gender != "F" && gender != "O")
            {
                return new OperationResult { Success = false, Message = "Gender must be 'M', 'F', or 'O'.", Object = null };
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

        public async Task<OperationResult> DisableUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found.", Object = null };
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

            return new OperationResult { Success = true, Message = "User disabled and future appointments deleted successfully.", Object = null };
        }

        public async Task<OperationResult> EnableUserAsync(int userId)
        {
            // Valida que el usuario exista
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found.", Object = null };
            }

            // Habilitar al usuario
            user.IsActive = true;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return new OperationResult { Success = true, Message = "User enabled successfully.", Object = null };
        }

        public async Task<OperationResult> AssignRolesAsync(int userId, List<int> roleIds)
        {
            // Valida si el usuario existe
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found", Object = null };
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

            return new OperationResult { Success = true, Message = "Roles assigned successfully.", Object = null };
        }

        public async Task<OperationResult> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new OperationResult { Success = false, Message = "User not found", Object = null };
            }

            // Verificar si el rol existe
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return new OperationResult { Success = false, Message = "Role not found", Object = null };
            }

            // Buscar la relación entre el usuario y el rol
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
            {
                return new OperationResult { Success = false, Message = "The user does not have this role assigned", Object = null };
            }

            // Eliminar la relación
            user.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return new OperationResult { Success = true, Message = "Role removed successfully.", Object = null };
        }

        private async Task<OperationResult> SaveChangesAsyncWithExceptionHandling()
        {
            try
            {
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Operation successful.", Object = null };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    var errorMessage = sqlEx.Message;
                    var index = errorMessage.IndexOf("'");
                    var columnName = errorMessage.Substring(index + 1, errorMessage.IndexOf("'", index + 1) - index - 1);
                    var errorMessageWithColumn = $"Duplicate entry for unique index. The value of '{columnName}' is already in use.";
                    return new OperationResult { Success = false, Message = errorMessageWithColumn, Object = null };
                }
                else
                {
                    throw;
                }
            }
        }

        // Metodo para convertir usuario a formato de respuesta
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
    }
}
