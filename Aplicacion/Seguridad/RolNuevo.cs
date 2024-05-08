using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Aplicacion.ManejadorError;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Aplicacion.Seguridad
{
    public class RolNuevo
    {
        public class Ejecuta : IRequest{
            public string Nombre { get; set; }
        }
        public class ValidaEjecuta: AbstractValidator<Ejecuta>
        {
            public ValidaEjecuta()
            {
                RuleFor(x => x.Nombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByIdAsync(request.Nombre);
                if (role != null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new { mensaje = "Ya existe el rol" });
                }
                var resultado = await _roleManager.CreateAsync(new IdentityRole(request.Nombre));
                if (resultado.Succeeded)
                {
                    return Unit.Value;

                }
                throw new Exception("No se pudo guardar el rol");
            }
        }
    }
}
