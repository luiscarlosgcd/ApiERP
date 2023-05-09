using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ApiERP.Models;

namespace ApiERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        public readonly ERPContext _ERPContext;

        public MonitorController(ERPContext erpContext)
        {
            _ERPContext = erpContext;
        }

        [HttpGet]
        [Route("erp")]
        [Authorize]
        public async Task<IActionResult> ErpGetAsync()
        {
            try
            {
                //Obtenemos Gasolineros con sus monitores designados
                var lista = await _ERPContext.Gasolineros
                    .Join(
                        _ERPContext.Monitors,
                        g => g.GasolineroId,
                        m => m.GasolineroId,
                        (g, m) => new {
                            GasolineroId = g.GasolineroId,
                            Nombre = g.Nombre,
                            MacAddress = m.MacAddress,
                            Fecha = m.Fecha,
                            IpAddress = m.IpAddress,
                            Tipo = m.Tipo,
                            Version = m.Version
                        }
                    )
                    .GroupBy(gm => new {
                        GasolineroId = gm.GasolineroId,
                        Nombre = gm.Nombre
                    })
                    .Select(gm => new {
                        GasolineroId = gm.Key.GasolineroId,
                        Nombre = gm.Key.Nombre,
                        Monitor = gm.Select(m => new {
                            MacAddress = m.MacAddress,
                            Fecha = m.Fecha,
                            IpAddress = m.IpAddress,
                            Tipo = m.Tipo,
                            Version = m.Version
                        }).ToList()
                    })
                    .ToListAsync();



                if (lista == null)
                {
                    return BadRequest("No se encontro monitores para consumir el servicio");
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        [Route("version")]
        public async Task<IActionResult> GetVersion()
        {
            try
            {
                
                var version = await _ERPContext.Componentes.FirstOrDefaultAsync(c => c.ComponenteId == 1);

                if (version == null)
                {
                    return BadRequest("No se encontro la version para consumir el servicio");
                }

                return Ok(version);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }
    }

}
