﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aplicacion.Comentarios;
using MediatR;
using System;

namespace WebApi.Controllers
{
    public class ComentarioController : MiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta { Id = id });   
        }
    }
}
