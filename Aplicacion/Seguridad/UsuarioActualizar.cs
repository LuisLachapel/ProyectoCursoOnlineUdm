﻿using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using FluentValidation;
using Persistencia;
using Aplicacion.Contratos;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Threading;
using Dominio;
using System.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Aplicacion.ManejadorError;

namespace Aplicacion.Seguridad
{
    public class UsuarioActualizar
    {
        public class Ejecuta: IRequest<UsuarioData>
        {
            public string NombreCompleto { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
            public ImagenGeneral ImagenPerfil { get; set; }
        }

        public class EjecutaValidador: AbstractValidator<Ejecuta>
        {
            public EjecutaValidador()
            {
                RuleFor(x => x.NombreCompleto).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IPasswordHasher<Usuario> _passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _passwordHasher = passwordHasher;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuarioIden = await _userManager.FindByNameAsync(request.Username);
                if(usuarioIden == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No existe un usuario con este username" });
                }

                var resultado = await _context.Users.Where(x => x.Email == request.Email && x.UserName != request.Username).AnyAsync();
                if (resultado)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new { mensaje = "Este email pertenece a otro usuario" });

                }

                if(request.ImagenPerfil != null)
                {
                    var resultadoImagen = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuarioIden.Id)).FirstOrDefaultAsync();
                    if(resultadoImagen == null)
                    {
                        var imagen = new Documento
                        {
                            Contenido = Convert.FromBase64String(request.ImagenPerfil.Data),
                            Nombre = request.ImagenPerfil.Nombre,
                            Extension = request.ImagenPerfil.Extension,
                            ObjetoReferencia = new Guid(usuarioIden.Id),
                            DocumentoId = Guid.NewGuid(),
                            FechaCreacion = DateTime.UtcNow
                        };
                        _context.Documento.Add(imagen);
                    }
                    else
                    {
                        resultadoImagen.Contenido = Convert.FromBase64String(request.ImagenPerfil.Data);
                        resultadoImagen.Nombre = request.ImagenPerfil.Nombre;
                        resultadoImagen.Extension = request.ImagenPerfil.Extension;
                        
                    }
                }

                usuarioIden.NombreCompleto = request.NombreCompleto;
                usuarioIden.PasswordHash = _passwordHasher.HashPassword(usuarioIden, request.Password);
                usuarioIden.Email = request.Email;

                var resultadoUpdate = await _userManager.UpdateAsync(usuarioIden);
                var resultadoRoles = await _userManager.GetRolesAsync(usuarioIden);
                var listRoles = new List<string>(resultadoRoles);

                var ImagenPerfil = await _context.Documento.Where(x => x.ObjetoReferencia == new Guid(usuarioIden.Id)).FirstOrDefaultAsync();
                ImagenGeneral imagenGeneral = null;
                if (ImagenPerfil != null)
                {
                    imagenGeneral = new ImagenGeneral
                    {
                        Data = Convert.ToBase64String(ImagenPerfil.Contenido),
                        Extension = ImagenPerfil.Extension,
                        Nombre = ImagenPerfil.Nombre
                    };
                }
                if (resultadoUpdate.Succeeded)
                {
                    return new UsuarioData
                    {
                        NombreCompleto = usuarioIden.NombreCompleto,
                        Username = usuarioIden.UserName,
                        Email = usuarioIden.Email,
                        Token = _jwtGenerador.CrearToken(usuarioIden,listRoles),
                        ImagenPerfil = imagenGeneral
                        
                    };
                }

                throw new Exception("No se pudo actualizar el usuario");
            }
        }
    }
}
