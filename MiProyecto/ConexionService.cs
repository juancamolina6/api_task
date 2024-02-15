// ConexionService.cs
using Npgsql;

namespace MiProyecto.Conexion
{
    public class ConexionService : IConexionService
    {
        private string _connectionString;

        public ConexionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }

}
