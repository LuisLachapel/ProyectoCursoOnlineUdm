using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Dominio;
using System.Threading.Tasks;
using System.Threading;
using Aplicacion.Contratos;
using Microsoft.AspNetCore.Identity;
using Persistencia;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta: IRequest<UsuarioData> { }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IUsuarioSesion _usuarioSesion;
            private readonly CursosOnlineContext _context;

            public Manejador(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IUsuarioSesion usuarioSesion, CursosOnlineContext context)
            {
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _usuarioSesion = usuarioSesion;
                _context = context;
            }
            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());
                var resultadoRoles = await _userManager.GetRolesAsync(usuario);
                var listaRoles = new List<string>(resultadoRoles);

                var ImagenPerfil = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();
                if (ImagenPerfil != null)
                {
                    var ImagenCliente = new ImagenGeneral
                    {
                        Data = Convert.ToBase64String(ImagenPerfil.Contenido),
                        Extension = ImagenPerfil.Extension,
                        Nombre = ImagenPerfil.Nombre

                    };

                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email,
                        ImagenPerfil = ImagenCliente
                    };
                }
                else
                {
                    return new UsuarioData
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Username = usuario.UserName,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email
                    };
                }
               
            }
        }
    }
}
