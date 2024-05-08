using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;
        public InstructorRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }

        public async Task<int> Actualiza(Guid InstructorId, string Nombre, string Apellidos, string Grado)
        {
            var storeProcedure = "usp_instructor_editar";
            try
            {
                var connection = _factoryConnection.GetConnection();
                var resultados = await connection.ExecuteAsync(
                storeProcedure,
                new
                {
                    instructorId = InstructorId,
                    nombre = Nombre,
                    Apellidos = Apellidos,
                    Grado = Grado

                },
                commandType : CommandType.StoredProcedure

                );

                _factoryConnection.CloseConnection();
                return resultados;
            }
            catch (Exception e)
            {

                throw new Exception("No se pudo insertar la data", e);
            }
        }

        public async Task<int> Elimina(Guid id)
        {
            var storeProcedure = "usp_instructor_elimina";
            try
            {
                var connection = _factoryConnection.GetConnection();
                var resultado = await connection.ExecuteAsync(
                storeProcedure,
                new
                {
                    Instructorid = id,
                },
                commandType: CommandType.StoredProcedure

                );
                _factoryConnection.CloseConnection();
                return resultado;
            }
            catch (Exception e)
            {

                throw new Exception("El instructor no se pudo eliminar", e);
            }
        }

        public async Task<int> Nuevo(string Nombre, string Apellidos, string Grado)
        {
            var storeProcedure = "usp_instructor_nuevo";
            try
            {
                var connection = _factoryConnection.GetConnection();
                var resultado = await connection.ExecuteAsync(storeProcedure, new
                {
                    InstructorId = Guid.NewGuid(),
                    Nombre = Nombre,
                    Apellidos = Apellidos,
                    Grado = Grado,


                },
                commandType: CommandType.StoredProcedure
                );

                _factoryConnection.CloseConnection();
                return resultado;   
            }
            catch (Exception e)
            {

                throw new Exception("No se pudo guardar el instructor", e);
            }
 
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList = null;
            var storeProcedure = "usp_Obtener_Instructores";
            try
            {
                var connection = _factoryConnection.GetConnection();
                instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new Exception("Error en la consulta de datos", e);
                throw;
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }
            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            var storeProcedure = "usp_Obtener_Instructor_por_id";
            InstructorModel instructor = null;
            try
            {
                var connection = _factoryConnection.GetConnection();
                instructor = await connection.QueryFirstAsync<InstructorModel> (
                    storeProcedure,
                    new
                    {
                        Id = id
                    },
                    commandType : CommandType.StoredProcedure
                );
                return instructor;
            }
            catch (Exception e)
            {

                throw new Exception("No se pudo encontrar el instructor", e);
            }
        }
    }
}
