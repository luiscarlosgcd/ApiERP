using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class SolicitudCambio
    {
        public int SolicitudCambioId { get; set; }
        public int ComponenteId { get; set; }
        public string? Md5 { get; set; }
        public string? Commit { get; set; }
        public int EstatusComponenteId { get; set; }
        public string? Version { get; set; }

        public virtual Componente Componente { get; set; } = null!;
        public virtual EstatusComponente EstatusComponente { get; set; } = null!;
    }
}
