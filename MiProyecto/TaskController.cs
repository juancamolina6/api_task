using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiProyecto.Conexion;
using MiProyecto.Models;


namespace MiProyecto
{
    public class TaskController : ControllerBase
    {
        private readonly IConexionService conexionService;

        public TaskController(IConexionService conexionService)
        {
            this.conexionService = conexionService;
        }

        [HttpGet("/tasks")]
        public async Task<ActionResult<IEnumerable<Dictionary<string, object>>>> GetTasks()
        {
            try
            {
                // Obtener la cadena de conexión a la base de datos PostgreSQL
                string connectionString = conexionService.GetConnectionString();
                
                // Crear una conexión a la base de datos PostgreSQL
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Ejecutar la consulta SQL para obtener las tareas
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM tasks", conn);
                using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                var tasksList = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var task = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetInt32(0),
                        ["title"] = reader.GetString(1),
                        ["description"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ["status"] = reader.GetString(3),
                        ["due_date"] = reader.GetDateTime(4),
                        ["user_id"] = reader.GetInt32(5),
                        ["create_date"] = reader.GetDateTime(6)
                    };
                    tasksList.Add(task);
                }

                return Ok(tasksList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las tareas: {ex.Message}");
            }
        }

        [HttpPost("/add_task")]
        public async Task<ActionResult> AddTask([FromBody] TaskDataModel taskData)
        {
            try
            {
                // Validar los datos recibidos
                if (taskData == null || string.IsNullOrEmpty(taskData.Title) || string.IsNullOrEmpty(taskData.Status))
                {
                    return BadRequest("Los datos de la tarea son inválidos.");
                }

                // Obtener la cadena de conexión a la base de datos PostgreSQL
                string sqlQuery = @"
                    INSERT INTO tasks (title, description, status, due_date, user_id)
                    VALUES (@title, @description, @status, @dueDate, @userId)";

                string connectionString = conexionService.GetConnectionString();

                // Crear una conexión a la base de datos PostgreSQL
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Ejecutar una consulta SQL para insertar la nueva tarea en la tabla 'tasks'
                using NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@title", taskData.Title);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(taskData.Description) ? DBNull.Value : taskData.Description);
                cmd.Parameters.AddWithValue("@status", taskData.Status);
                cmd.Parameters.AddWithValue("@dueDate", taskData.DueDate);
                cmd.Parameters.AddWithValue("@userId", taskData.UserId);

                await cmd.ExecuteNonQueryAsync();

                // La tarea se agregó correctamente, devolvemos un código de estado 200 (OK)
                return Ok("Tarea agregada correctamente.");
            }
            catch (Exception ex)
            {
                // Si hay un error, devolvemos un código de estado 500 (Error interno del servidor) con un mensaje de error
                return StatusCode(500, $"Error al agregar la tarea: {ex.Message}");
            }
        }

        [HttpPut("/edit_task/{id_task}")]
        public async Task<ActionResult> EditTask(int id_task, [FromBody] Dictionary<string, object> taskData)
        {
            try
            {
                // Extraer los datos de la tarea del cuerpo de la solicitud HTTP
                var title = taskData["title"].ToString();
                var description = taskData.ContainsKey("description") ? taskData["description"].ToString() : null;
                var status = taskData["status"].ToString();
                var dueDate = DateTime.Parse(taskData["due_date"].ToString());

                // Obtener la cadena de conexión a la base de datos PostgreSQL
                string connectionString = conexionService.GetConnectionString();
                
                // Crear una conexión a la base de datos PostgreSQL
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Ejecutar una consulta SQL para actualizar los datos de la tarea en la tabla 'tasks' según el 'id_task' proporcionado
                using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE tasks SET title = @title, description = @description, status = @status, due_date = @dueDate WHERE id_task = @idTask", conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@dueDate", dueDate);
                cmd.Parameters.AddWithValue("@idTask", id_task);

                await cmd.ExecuteNonQueryAsync();

                // La tarea se editó correctamente, devolvemos un código de estado 200 (OK)
                return Ok("Tarea editada correctamente.");
            }
            catch (Exception ex)
            {
                // Si hay un error, devolvemos un código de estado 500 (Error interno del servidor) con un mensaje de error
                return StatusCode(500, $"Error al editar la tarea: {ex.Message}");
            }
        }


        [HttpDelete("/delete_task/{id_task}")]
        public async Task<ActionResult> DeleteTask(int id_task)
        {
            try
            {
                // Obtener la cadena de conexión a la base de datos PostgreSQL
                string connectionString = conexionService.GetConnectionString();
                
                // Crear una conexión a la base de datos PostgreSQL
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Ejecutar una consulta SQL para eliminar la tarea correspondiente de la tabla 'tasks' según el 'id_task' proporcionado
                using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM tasks WHERE id_task = @idTask", conn);
                cmd.Parameters.AddWithValue("@idTask", id_task);

                await cmd.ExecuteNonQueryAsync();

                // La tarea se eliminó correctamente, devolvemos un código de estado 200 (OK)
                return Ok("Tarea eliminada correctamente.");
            }
            catch (Exception ex)
            {
                // Si hay un error, devolvemos un código de estado 500 (Error interno del servidor) con un mensaje de error
                return StatusCode(500, $"Error al eliminar la tarea: {ex.Message}");
            }
        }


       [HttpGet("/search_task/{id}")]
        public async Task<ActionResult> SearchTask(int id)
        {
            try
            {
                // Obtener la cadena de conexión a la base de datos PostgreSQL
                string connectionString = conexionService.GetConnectionString();
                
                // Crear una conexión a la base de datos PostgreSQL
                using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Ejecutar una consulta SQL para buscar la tarea correspondiente en la tabla 'tasks' según el 'id' proporcionado
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM tasks WHERE id_task = @idTask", conn);
                cmd.Parameters.AddWithValue("@idTask", id);

                // Ejecutar la consulta y obtener el resultado
                using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                // Verificar si se encontró una tarea con el ID proporcionado
                if (await reader.ReadAsync())
                {
                    // Construir un objeto de tarea con los datos de la consulta
                    var task = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetInt32(0),
                        ["title"] = reader.GetString(1),
                        ["description"] = reader.IsDBNull(2),
                        ["status"] = reader.GetString(3),
                        ["due_date"] = reader.GetDateTime(4),
                        ["user_id"] = reader.GetInt32(5),
                        ["create_date"] = reader.GetDateTime(6)
                    };

                    // Devolver la tarea encontrada
                    return Ok(task);
                }
                else
                {
                    // Si no se encontró ninguna tarea con el ID proporcionado, devolver un mensaje de error
                    return NotFound("No se encontró ninguna tarea con el ID especificado.");
                }
            }
            catch (Exception ex)
            {
                // Si hay un error, devolvemos un código de estado 500 (Error interno del servidor) con un mensaje de error
                return StatusCode(500, $"Error al buscar la tarea: {ex.Message}");
            }
        }

    }
}

