using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia;
using Aplicacion.ManejadorError;
using System.Net;
using System.Linq;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta: IRequest
        {
            public Guid Id { get; set; }

            public class Manejador : IRequestHandler<Ejecuta>
            {
                private readonly CursosOnlineContext _context;
                public Manejador(CursosOnlineContext context)
                {
                    _context = context;
                }

                public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
                {
                    var InstructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.Id);
                    foreach (var instructor in InstructoresDB)
                    {
                        _context.CursoInstructor.Remove(instructor);
                    }

                    var ComentarioDB = _context.Comentario.Where(x => x.CursoId == request.Id);
                    foreach( var cmt in ComentarioDB)
                    {
                        _context.Comentario.Remove(cmt);

                    }

                    var PrecioDB = _context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();
                    if (PrecioDB!= null)
                    {
                        _context.Precio.Remove(PrecioDB);
                    }



                    var curso = await _context.Curso.FindAsync(request.Id);
                    if(curso == null)
                    {
                        //throw new Exception("No se pudo eliminar el curso");
                        throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});

                    }
                    _context.Remove(curso);

                    var resultado = await _context.SaveChangesAsync();
                    if (resultado > 0)
                    {
                        return Unit.Value;

                    }
                    throw new Exception("No se pudieron guardar los cambios");
                }
            }
        }
    }
}
