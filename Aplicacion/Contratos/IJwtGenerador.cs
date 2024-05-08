using System;
using System.Collections.Generic;
using System.Text;
using Dominio;

namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
        string CrearToken(Usuario usuario, List<string> roles);
    }
}
