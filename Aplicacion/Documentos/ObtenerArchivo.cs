using System;
using System.Collections.Generic;
using System.Text;
using Persistencia;
using Dominio;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Aplicacion.ManejadorError;
using System.Net;

namespace Aplicacion.Documentos
{
    public class ObtenerArchivo
    {
       public class Ejecuta: IRequest<ArchivoGenerico>
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, ArchivoGenerico>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<ArchivoGenerico> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var archivo = await _context.Documento.Where(x => x.ObjetoReferencia == request.Id).FirstOrDefaultAsync();
                if(archivo == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro la imagen" });
                }

                var archivoGenerico = new ArchivoGenerico
                {
                    Data = Convert.ToBase64String(archivo.Contenido),
                    Nombre = archivo.Nombre,
                    Extension = archivo.Extension
                };

                return archivoGenerico;
            }
        }
    }
}
