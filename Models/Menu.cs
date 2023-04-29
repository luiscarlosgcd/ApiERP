using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class Menu
    {
        public int MenuId { get; set; }
        public string Descripcion { get; set; } = null!;
        public int PadreId { get; set; }
        public int Orden { get; set; }
    }
}
