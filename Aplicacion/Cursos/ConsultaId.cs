using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Dominio;
using Persistencia;
using System.Threading.Tasks;
using System.Threading;
using Aplicacion.ManejadorError;
using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoUnico: IRequest<CursoDto>
        {
            public Guid Id { get; set; }
        }

        public class Manejador: IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;
            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CursoDto> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                var curso = await _context.Curso.Include(x => x.ComentarioLista).Include(x => x.PrecioPromocion).Include(x => x.InstructoresLink ).ThenInclude(y => y.Instructor).FirstOrDefaultAsync(a => a.CursoId == request.Id);
                if (curso == null)
                {
                    //throw new Exception("El curso no existe");
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new { mensaje = "No se encontro el curso" });
                }
                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);
                return cursoDto;
            }
        }
    }
}
