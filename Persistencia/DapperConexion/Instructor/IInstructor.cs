using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
        Task<IEnumerable<InstructorModel>> ObtenerLista();
        Task<InstructorModel> ObtenerPorId(Guid id);
        Task<int> Nuevo(string Nombre, string Apellidos, string Grado);
        Task<int> Actualiza(Guid instructorId, string Nombre, string Apellidos, string Grado);
        Task<int> Elimina(Guid id);
    }
}
