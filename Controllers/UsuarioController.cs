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
        private readonly IConfiguration _configuration;

        public IConfiguration Configuration { get; }

        public UsuarioController(IUsuarioService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
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

            string token = new Authentication(_configuration).GenerateTokenJwt(JsonSerializer.Serialize<DtoUserInfo>(userInfo));
            return Ok(new
            {
                AccesToken = token
            });
        }
    }
}