using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Subsistema_Estudiantil.Models;
using System.Data;

namespace Subsistema_Estudiantil.Servicios
{
    public interface IRepositorioEstudiante
    {
        Task ActualizarE(FormularioEstudiantes formularioEstudiantes);
        Task EliminarE(FormularioEstudiantes formularioEstudiantes);
        Task InsertEstudiante(FormularioEstudiantes formularioestudiantes);
        Task<IEnumerable<FormularioEstudiantes>> ObtenerEstudiante(int idusuario);
        Task<IEnumerable<FormularioEstudiantes>> TablaEstudianteCancelada(int idUsuario);
        Task<FormularioEstudiantes> ObtenerIdRegistroE(int idP);
        Task Ordenar(IEnumerable<FormularioEstudiantes> formularioestudianteactualizacion);
        Task DetalleEC(FormularioEstudiantes formularioEstudiantes);
        Task<FormularioEstudiantes> ObtenerIdRegistroEC(int id);
        Task DetalleE(FormularioEstudiantes formularioEstudiantes);
        //IEnumerable<FormularioCarrera> ObtenerListado();
    }
    public class RepositorioEstudiante: IRepositorioEstudiante
    {
        private readonly string connectionstring;
        private readonly IRepositorioMaestras repositorioMaestras;

        public RepositorioEstudiante(IConfiguration configuration,IRepositorioMaestras repositorioMaestras)
        {
            connectionstring = configuration.GetConnectionString("DefaultConnection");
            this.repositorioMaestras = repositorioMaestras;
        }

        public async Task InsertEstudiante(FormularioEstudiantes formularioestudiantes)
        {
            using(SqlConnection connection = new SqlConnection(connectionstring))
            {
                var id = await connection.QuerySingleAsync<int>("InsertEstudiante_2", 
                                                                new {nombre = formularioestudiantes.Nombre,
                                                                apellido = formularioestudiantes.Apellido,
                                                                carrera = formularioestudiantes.Carrera,
                                                                gmail = formularioestudiantes.Gmail,
                                                                documento = formularioestudiantes.Documento,
                                                                nacimiento = formularioestudiantes.Nacimiento,
                                                                observacion = formularioestudiantes.Observacion,
                                                                idusuario = formularioestudiantes.IdUsuario},
                                                                commandType: System.Data.CommandType.StoredProcedure);
                formularioestudiantes.IdEstudiante = id;
            }
        }
        //Obtiene las solicitudes pendientes de los estudiantes para ser listadas 
        public async Task<IEnumerable<FormularioEstudiantes>> ObtenerEstudiante(int idUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(connectionstring))
            {
                return await conexion.QueryAsync<FormularioEstudiantes>(@"SELECT * FROM Estudiante
                                                                        WHERE IdUsuario = @IdUsuario
                                                                        ORDER BY Orden",new {IdUsuario=idUsuario});
            }
        }

        public async Task<IEnumerable<FormularioEstudiantes>> TablaEstudianteCancelada(int idUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(connectionstring))
            {
                return await conexion.QueryAsync<FormularioEstudiantes>(@"SELECT * FROM EstudianteCancelada
                                                                        WHERE IdUsuario = @IdUsuario
                                                                        ORDER BY Orden", new { IdUsuario = idUsuario });
            }
        }


        public async Task ActualizarE(FormularioEstudiantes formularioEstudiantes)
        {
            using(SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("UPDATE Estudiante SET Nombre=@Nombre,Apellido=@Apellido," +
                                             "Carrera=@Carrera,Gmail=@Gmail,Documento=@Documento" +
                                             ",Nacimiento=@Nacimiento,Observacion=@Observacion " +
                                             "WHERE IdEstudiante=@IdEstudiante;", formularioEstudiantes);

            }
        }
        //Obtiene el id del registro para realizar su actualizacion e imprimir
        //la informacion el el formulario de actualizacion
        public async Task<FormularioEstudiantes> ObtenerIdRegistroE(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioEstudiantes>(
                    @"SELECT * FROM Estudiante WHERE IdEstudiante = @IdEstudiante",
                    new { IdEstudiante = id });
            }
        }

        public async Task EliminarE(FormularioEstudiantes formularioEstudiantes)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("InsertEstudianteCancelada_2",
                               new
                               {
                                   nombre = formularioEstudiantes.Nombre,
                                   apellido = formularioEstudiantes.Apellido,
                                   carrera = formularioEstudiantes.Carrera,
                                   gmail = formularioEstudiantes.Gmail,
                                   documento = formularioEstudiantes.Documento,
                                   nacimiento = formularioEstudiantes.Nacimiento,
                                   observacion = formularioEstudiantes.Observacion,
                                   idusuario = formularioEstudiantes.IdUsuario
                               }, commandType: System.Data.CommandType.StoredProcedure);
            }
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("DELETE Estudiante WHERE IdEstudiante=@IdEstudiante",
                                               formularioEstudiantes);
            }
        }
        //Se obtiene la informacion como si se fuese actualizar para rellenar los campos pero se inhabilita porque 
        //solo se quiere el detalle de la misma entonces seria una forma de rellenar los campos. 
        public async Task DetalleEC(FormularioEstudiantes formularioEstudiantes)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("UPDATE EstudianteCancelada SET Nombre=@Nombre,Apellido=@Apellido," +
                                             "Carrera=@Carrera,Gmail=@Gmail,Documento=@Documento" +
                                             ",Nacimiento=@Nacimiento,Observacion=@Observacion " +
                                             "WHERE IdEstudiante=@IdEstudiante;", formularioEstudiantes);
            }
        }
        public async Task DetalleE(FormularioEstudiantes formularioEstudiantes)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("UPDATE Estudiante SET Nombre=@Nombre,Apellido=@Apellido," +
                                             "Carrera=@Carrera,Gmail=@Gmail,Documento=@Documento" +
                                             ",Nacimiento=@Nacimiento,Observacion=@Observacion " +
                                             "WHERE IdEstudiante=@IdEstudiante;", formularioEstudiantes);
            }
        }
        //obtiene el id de los registros de las solicitudes canceladas
        public async Task<FormularioEstudiantes> ObtenerIdRegistroEC(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioEstudiantes>(
                    @"SELECT * FROM EstudianteCancelada WHERE IdEstudiante = @IdEstudiante",
                    new { IdEstudiante = id });
            }
        }
        public async Task Ordenar(IEnumerable<FormularioEstudiantes> formularioestudianteactualizacion)
        {
            var query = "UPDATE Estudiante SET Orden=@Orden WHERE IdEstudiante=@IdEstudiante";
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync(query, formularioestudianteactualizacion);
            }
        }
    }
}
