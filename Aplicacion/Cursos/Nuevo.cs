using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Persistencia;
using Dominio;
using FluentValidation;
namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta: IRequest
        {
            public Guid ? CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime FechaPublicacion { get; set; }
            public List<Guid> ListaInstructor { get; set; }
            public Decimal Precio { get; set; }
            public Decimal Promocion { get; set; }
        }

        public class EjecutaValidacion: AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                Guid _CursoId = Guid.NewGuid();
                if(request.CursoId != null)
                {
                    _CursoId = request.CursoId ?? Guid.NewGuid();
                }
                var curso = new Curso
                {
                    CursoId = _CursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow
                };



                _context.Curso.Add(curso);

                if(request.ListaInstructor != null)
                {
                    foreach (var id in request.ListaInstructor)
                    {
                        var cursoInstructor = new CursoInstructor
                        {
                            CursoId = _CursoId,
                            InstructorId = id

                        };
                        _context.CursoInstructor.Add(cursoInstructor);

                    }
                }

                /**/
                var precioEntidad = new Precio{ 
                    CursoId = _CursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid(),
                };
                _context.Precio.Add(precioEntidad);

                var valor = await _context.SaveChangesAsync();
                if (valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}
