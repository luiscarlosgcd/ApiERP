using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class EstatusComponente
    {
        public EstatusComponente()
        {
            SolicitudCambios = new HashSet<SolicitudCambio>();
        }

        public int EstatusComponenteId { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<SolicitudCambio> SolicitudCambios { get; set; }
    }
}
