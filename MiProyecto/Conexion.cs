using System;
using Npgsql;

namespace MiProyecto.Conexion
{
    public class Conexion
    {
        private static string connectionString = "Host=127.0.0.1;Username=postgres;Password=1234567890;Database=list_tasks;Port=5432";

        public static NpgsqlConnection Conect()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            try
            {
                conn.Open();
                Console.WriteLine("Conexión a la base de datos exitosa.");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
            }
            return conn;
        }

        public static void CreateTables(NpgsqlConnection conn)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
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
                    Console.WriteLine("Tabla 'users' creada correctamente.");

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
                    Console.WriteLine("Tabla 'tasks' creada correctamente.");
                }
                Console.WriteLine("Operaciones de creación completadas.");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Error al crear las tablas: " + ex.Message);
            }
        }
    }
}
