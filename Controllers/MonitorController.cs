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
                //var lista = await _erpContext.Clientes.Include(c => c.Monitors).ToListAsync();
                //var lista = await _ERPContext.Gasolineros.Include(c => c.Monitors).AsSplitQuery().ToListAsync();
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

    }
}
