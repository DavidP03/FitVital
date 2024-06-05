using FitVital.DAL.Entities;
using FitVital.DAL;
using fitVital_API.DAL.Entities;
using fitVital_API.Domain.Interfaces;
using fitVital_API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.DAL;

namespace fitVital_API.Domain.Services
{
    public class EntrenadorService : IEntrenadorService
    {
        private readonly DataBaseContext _context;

        public EntrenadorService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Entrenador> DesactivarEntrenador(Guid id)
        {
            try
            {

                var entrenador = await _context.Entrenadores.FirstOrDefaultAsync(x => x.EntrenadorId == id);

                if (entrenador == null)
                {
                    throw new Exception("Entrenador no encontrado");
                }

                if (entrenador.Activo == false)
                {
                    throw new Exception("Entrenador ya se encuentra desactivado");
                }

                entrenador.Activo = false;

                await _context.SaveChangesAsync();

                return entrenador;


            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }

        public async Task<IEnumerable<Entrenador>> GetEntrenadores()
        {
            try
            {
                var entrenadores = await _context.Entrenador.ToList();

                return < IEnumerable < Entrenador >> entrenadores;

            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }

        public async Task<Entrenador> PostEntrenador(Entrenador entrenador)
        {
            try
            {
                entrenador.EntrenadorId = Guid.NewGuid();
                entrenador.Nombre = entrenador.Nombre;
                entrenador.Activo = true;

                _context.Entrenadores.Add(entrenador);


                await _context.SaveChangesAsync();

                return entrenador;

            }
            catch (DbUpdateException dbUpdateException)
            {
                throw new Exception(dbUpdateException.InnerException?.Message ?? dbUpdateException.Message);
            }
        }
    }
}
