using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ApiERP.Models;
using ApiERP.Middlewares;
using ApiERP.Dto.Request;
using ApiERP.Dto;
using ApiERP.Services;

namespace ApiERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;


        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DtoAuthRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _service.GetAsync(login);
            if(user == null)
            {
                return NotFound();
            }

            var userInfo = new DtoUserInfo
            {
                GasolineroId = user.UsuarioId,
                Nombre = user.Usuario1
            };

            string token = new Authentication().GenerateTokenJwt(JsonSerializer.Serialize<DtoUserInfo>(userInfo));
            return Ok(new
            {
                AccesToken = token
            });
        }
    }
}