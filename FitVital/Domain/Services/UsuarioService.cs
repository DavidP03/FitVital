using FitVital.DAL.Entities;
using FitVital.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WebAPI.DAL;

namespace FitVital.Domain.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataBaseContext _context;

        public UsuarioService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
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

        public async Task<Usuario> GetUsuarioByIdAsync(Guid id)
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

        public async Task<Usuario> PostUsuarioAsync(Usuario usuario)
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

        public async Task<Usuario> DeleteUsuarioAsync(Guid id)
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
    }
}
