using Dapper;
using Microsoft.Data.SqlClient;
using Subsistema_Estudiantil.Models;

namespace Subsistema_Estudiantil.Servicios
{
    public interface IRepositorioProfesor
    {
        Task InsertProfesor(FormularioProfesor formularioProfesor);
        Task Actualizar(FormularioProfesor formularioProfesor);
        Task<IEnumerable<FormularioProfesor>> Obtener(int idUsuario);
        Task<FormularioProfesor> ObtenerIdRegistro(int id);
        Task Eliminar(FormularioProfesor formularioProfesor);
        Task Ordenar(IEnumerable<FormularioProfesor> formularioprofesoractualizacion);
        Task<IEnumerable<FormularioProfesor>> TablaProfesorCancelada(int usuario);
        Task DetallePC(FormularioProfesor formularioprofesor);
        Task<FormularioProfesor> ObtenerIdRegistroPC(int id);
        Task<FormularioProfesor> ObtenerIdRegistroP(int id);
        Task DetalleP(FormularioProfesor formularioprofesor);
    }
    public class RepositorioProfesor: IRepositorioProfesor
    {
        private readonly string connectionString;
        public RepositorioProfesor(IConfiguration configuracion)
        {
            connectionString = configuracion.GetConnectionString("DefaultConnection");
        }

        public async Task InsertProfesor(FormularioProfesor formularioProfesor)
        {
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("FormularioProfesor_Insertar",
                                                        new {nombre = formularioProfesor.Nombre,
                                                        apellido=formularioProfesor.Apellido,
                                                        asignatura=formularioProfesor.Asignatura,
                                                        gmail=formularioProfesor.Gmail,
                                                        documento=formularioProfesor.Documento,
                                                        nacimiento=formularioProfesor.Nacimiento,
                                                        observacion=formularioProfesor.Observacion,
                                                        idusuario = formularioProfesor.IdUsuario},
                                                        commandType: System.Data.CommandType.StoredProcedure);


                formularioProfesor.IdProfesor = id;

            }
        }

        public async Task<IEnumerable<FormularioProfesor>> TablaProfesorCancelada(int usuario)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                return await conexion.QueryAsync<FormularioProfesor>(@"SELECT * FROM ProfesorCancelada
                                                                        WHERE IdUsuario = @IdUsuario
                                                                        ORDER BY Orden", new { IdUsuario = usuario });
            }
        }
        public async Task<IEnumerable<FormularioProfesor>> Obtener(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return await connection.QueryAsync<FormularioProfesor>(@"SELECT * FROM Profesor 
                                                                        WHERE IdUsuario = @IdUsuario
                                                                        ORDER BY Orden",new {Idusuario = idUsuario});
            }
        }

        public async Task Actualizar(FormularioProfesor formularioProfesor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("UPDATE Profesor SET Nombre=@Nombre,Apellido=@Apellido," +
                                              "Asignatura=@Asignatura,Gmail=@Gmail,Documento=@Documento" +
                                              ",Nacimiento=@Nacimiento," +
                                              "Observacion=@Observacion WHERE IdProfesor=@IdProfesor;",
                                              formularioProfesor);
            }
        }
        //Obtiene el id del registro para realizar su actualizacion e imprimir
        //la informacion el el formulario de actualizacion
        public async Task<FormularioProfesor> ObtenerIdRegistro(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioProfesor>(@"SELECT * FROM
                                                            Profesor WHERE IdProfesor=@IdProfesor", 
                                                            new { IdProfesor = id });
                                                                                                               
            }
        }

        public async Task Eliminar(FormularioProfesor formularioProfesor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("InsertarProfesorCancelada_2",
                               new
                               {
                                   nombre = formularioProfesor.Nombre,
                                   apellido = formularioProfesor.Apellido,
                                   asignatura = formularioProfesor.Asignatura,
                                   gmail = formularioProfesor.Gmail,
                                   documento = formularioProfesor.Documento,
                                   nacimiento = formularioProfesor.Nacimiento,
                                   observacion = formularioProfesor.Observacion,
                                   idusuario = formularioProfesor.IdUsuario
                               }, commandType: System.Data.CommandType.StoredProcedure);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("DELETE Profesor WHERE IdProfesor=@IdProfesor",
                                               formularioProfesor);
            }
        }

        public async Task Ordenar(IEnumerable<FormularioProfesor> formularioprofesoractualizacion)
        {
            var query = "UPDATE Profesor SET Orden=@Orden WHERE IdProfesor=@IdProfesor";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(query,formularioprofesoractualizacion);
            }
        }
        //obtiene el id de los registros de las solicitudes canceladas
        public async Task<FormularioProfesor> ObtenerIdRegistroPC(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioProfesor>(
                    @"SELECT * FROM ProfesorCancelada WHERE IdProfesor = @IdProfesor",
                    new { IdProfesor = id });
            }
        }
        //obtiene el id de los registros de las solicitudes 
        public async Task<FormularioProfesor> ObtenerIdRegistroP(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioProfesor>(
                    @"SELECT * FROM Profesor WHERE IdProfesor = @IdProfesor",
                    new { IdProfesor = id });
            }
        }
        public async Task DetallePC(FormularioProfesor formularioprofesor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("UPDATE ProfesorCanceada SET Nombre=@Nombre,Apellido=@Apellido," +
                                             "Carrera=@Carrera,Gmail=@Gmail,Documento=@Documento" +
                                             ",Nacimiento=@Nacimiento,Observacion=@Observacion " +
                                             "WHERE IdProfesor=@IdProfesor;", formularioprofesor);
            }
        }

        public async Task DetalleP(FormularioProfesor formularioprofesor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("UPDATE Profesor SET Nombre=@Nombre,Apellido=@Apellido," +
                                             "Asignatura=@Asignatura,Gmail=@Gmail,Documento=@Documento" +
                                             ",Nacimiento=@Nacimiento,Observacion=@Observacion " +
                                             "WHERE IdEstudiante=@IdEstudiante;", formularioprofesor);
            }
        }
    }
}
