using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class Componente
    {
        public Componente()
        {
            SolicitudCambios = new HashSet<SolicitudCambio>();
        }

        public int ComponenteId { get; set; }
        public string Descripcion { get; set; } = null!;
        public string? Version { get; set; }

        public virtual ICollection<SolicitudCambio> SolicitudCambios { get; set; }
    }
}
