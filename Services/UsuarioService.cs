using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApiERP.Models;
using ApiERP.Dto.Request;

namespace ApiERP.Services
{
    public class UsuarioService:IUsuarioService
    {
        public async Task<Usuario> GetAsync(DtoAuthRequest login)
        {
            using (var db = new ERPContext())
            {
                var user = db.Usuarios.Where(d => d.Usuario1 == login.Usuario &&
                                                d.Clave == login.Password).FirstOrDefault();
                if (user == null) return null;

                return user;
            }
            return null;
        }
    }
}
