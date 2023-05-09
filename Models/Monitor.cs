using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class Monitor
    {
        public string MacAddress { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Version { get; set; } = null!;
        public int GasolineroId { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Gasolinero Gasolinero { get; set; } = null!;
    }
}
