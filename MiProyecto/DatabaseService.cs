using Npgsql;
using MiProyecto.Interfaces;
using MiProyecto.Conexion;


namespace MiProyecto.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IConexionService _conexionService;

        public DatabaseService(IConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        public void CreateTables()
        {
            string connectionString = _conexionService.GetConnectionString();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Crear la tabla 'users' si no existe
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS users (
                    user_id SERIAL PRIMARY KEY,
                    name VARCHAR(100),
                    cc int,
                    email VARCHAR(100)
                )";
            cmd.ExecuteNonQuery();

            // Crear la tabla 'tasks' si no existe
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    id_task SERIAL PRIMARY KEY,
                    title VARCHAR(255),
                    description TEXT,
                    status VARCHAR(20),
                    due_date DATE,
                    user_id SERIAL REFERENCES users(user_id),
                    crate_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )";
            cmd.ExecuteNonQuery();
        }
    }
}
