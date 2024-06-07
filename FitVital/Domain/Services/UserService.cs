using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
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

        public async Task<User> RegisterUserAsync(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task DeactivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }
    }

    /*public class UserService : IUserService
    {
        private readonly DataBaseContext _context;

        public UserService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsuariosAsync()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return usuarios;
            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }

        public async Task<User> GetUsuarioByIdAsync(Guid id)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(c => c.UsuarioId == id);

                return usuario;
            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }

        public async Task<User> PostUsuarioAsync(User usuario)
        {
            try
            {
                usuario.UsuarioId = Guid.NewGuid();
                usuario.Nombre = usuario.Nombre;
                usuario.Telefono = usuario.Telefono;
                usuario.Correo = usuario.Correo;
                usuario.Edad = usuario.Edad;
                usuario.Genero = usuario.Genero;
                _context.Usuarios.Add(usuario); //El Método Add() me permite crear el objeto en el contexto de mi BD

                await _context.SaveChangesAsync();

                return usuario;

            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception("Error a la hora de insertar");
            }
        }

        public async Task<User> DeleteUsuarioAsync(Guid id)
        {
            try
            {
                var usuario = await GetUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return null;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync(); //La ejecución del Query

                return usuario;
            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }
    }*/
}
