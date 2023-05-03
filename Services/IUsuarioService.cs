using ApiERP.Models;
using ApiERP.Dto.Request;

namespace ApiERP.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> GetAsync(DtoAuthRequest login);
    }
}
