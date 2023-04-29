using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class UsuarioGasolinero
    {
        public int UsuarioId { get; set; }
        public int GasolineroId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
