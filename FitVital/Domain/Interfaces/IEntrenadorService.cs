using FitVital.DAL.Entities;

namespace fitVital_API.Domain.Interfaces
{
    public interface IEntrenadorService
    {
        Task<IEnumerable<Entrenador>> GetEntrenadores();
        Task<Entrenador> PostEntrenador(Entrenador entrenador);
        Task<Entrenador> DesactivarEntrenador(Guid id);

    }
}
