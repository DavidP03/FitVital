using FitVital.DAL.Entities;

namespace FitVital.Domain.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetUsuariosAsync();
        Task<Usuario> GetUsuarioByIdAsync(Guid id);
        Task<Usuario> PostUsuarioAsync(Usuario usuario);
        Task<Usuario> DeleteUsuarioAsync(Guid id);
    }
}
