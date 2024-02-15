// IConexionService.cs
using Npgsql;

namespace MiProyecto.Conexion
{
    public interface IConexionService
    {
        string GetConnectionString();
    }

}