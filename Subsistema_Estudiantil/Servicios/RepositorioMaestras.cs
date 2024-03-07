using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Subsistema_Estudiantil.Models;

namespace Subsistema_Estudiantil.Servicios
{
    public interface IRepositorioMaestras
    {
        Task EliminarCarrera(FormularioCarrera formulariocarrera);
        Task InsertCarrera(FormularioCarrera formulariocarrera);
        Task<IEnumerable<FormularioCarrera>> ObtenerCarrera(int id);
        Task<FormularioCarrera> ObtenerIdRegistroC(int id);
    }
    public class RepositorioMaestras : IRepositorioMaestras
    {
        private readonly string connectionstring;
        public RepositorioMaestras(IConfiguration configuration)
        {
            connectionstring = configuration.GetConnectionString("DefaultConnection");
        }
        //Obtiene las carreras para ser listadas
        public async Task<IEnumerable<FormularioCarrera>> ObtenerCarrera(int id)
        {
            using (SqlConnection conexion = new SqlConnection(connectionstring))
            {
                return await conexion.QueryAsync<FormularioCarrera>(@"SELECT * FROM Carrera
                                                                        WHERE UsuarioId = @UsuarioId", new { UsuarioId = id });
            }
        }

        public async Task InsertCarrera(FormularioCarrera formulariocarrera)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                var id = await connection.QuerySingleAsync<int>(@"Insert into Carrera(Carrera,UsuarioId)
                                                                Values(@Carrera,@UsuarioId);
                                                                SELECT SCOPE_IDENTITY()", formulariocarrera);
                                                                
                formulariocarrera.Id = id;
            }
        }
        //Obtiene el Id de la solicitud para eliminar la carrera. 
        public async Task<FormularioCarrera> ObtenerIdRegistroC(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                return await connection.QueryFirstOrDefaultAsync<FormularioCarrera>(
                    @"SELECT * FROM Carrera WHERE Id = @Id",
                    new { Id = id });
            }
        }

        public async Task EliminarCarrera(FormularioCarrera formulariocarrera)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.ExecuteAsync("DELETE Carrera WHERE Id = @Id",
                                               formulariocarrera);
            }
        }
    }
}
