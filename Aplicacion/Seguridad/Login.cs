using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Dominio;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Aplicacion.ManejadorError;
using System.Net;
using FluentValidation;
using Aplicacion.Contratos;
using Persistencia;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Seguridad
{
    public class Login
    {
        public class Ejecuta: IRequest<UsuarioData>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly SignInManager<Usuario> _signInManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly CursosOnlineContext _context;
            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtGenerador jwtGenerador, CursosOnlineContext context)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jwtGenerador = jwtGenerador;
                _context = context;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByEmailAsync(request.Email);
                if(usuario == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
                }
                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                var resultadoRoles = await _userManager.GetRolesAsync(usuario);
                var listaRoles = new List<string>(resultadoRoles);

                var ImagenPerfil = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuario.Id)).FirstOrDefaultAsync();
                if (resultado.Succeeded)
                {
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
                            Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                            Username = usuario.UserName,
                            Email = usuario.Email,
                            ImagenPerfil = ImagenCliente
                        };

                    }
                    else
                    {
                        return new UsuarioData
                        {
                            NombreCompleto = usuario.NombreCompleto,
                            Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                            Username = usuario.UserName,
                            Email = usuario.Email
                        };
                    }
                }
               
                throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
            }
        }
    }
}
