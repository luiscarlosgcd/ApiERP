using System;
using System.Collections.Generic;

namespace ApiERP.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            UsuarioGasolineros = new HashSet<UsuarioGasolinero>();
        }

        public int UsuarioId { get; set; }
        public string Usuario1 { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public bool Administrador { get; set; }

        public virtual ICollection<UsuarioGasolinero> UsuarioGasolineros { get; set; }
    }
}
