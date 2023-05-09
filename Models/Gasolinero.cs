using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class Gasolinero
    {
        public Gasolinero()
        {
            Monitors = new HashSet<Monitor>();
        }

        public int GasolineroId { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Monitor> Monitors { get; set; }
    }
}
