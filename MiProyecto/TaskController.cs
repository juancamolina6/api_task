using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiProyecto.Conexion;

namespace MiProyecto
{
    
    public class TaskController : ControllerBase
    {
        [HttpGet("/tasks")]
        public async Task<ActionResult<IEnumerable<Dictionary<string, object>>>> GetTasks()
        {
            // Conexion a la base de datos PostgreSQL
            NpgsqlConnection conn= Conexion.Conect();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand("SELECT * FROM tasks", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

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

        [HttpPost("/add_task")]
        public async Task<ActionResult> AddTask([FromBody] Dictionary<string, object> taskData)
        {
            try
            {
                // Extraer los datos de la tarea del cuerpo de la solicitud HTTP
                var title = taskData["title"].ToString();
                var description = taskData.ContainsKey("description") ? taskData["description"].ToString() : null;
                var status = taskData["status"].ToString();
                var dueDate = DateTime.Parse(taskData["due_date"].ToString());
                var userId = Convert.ToInt32(taskData["user_id"]);

                // Conexion a la base de datos PostgreSQL
                using var conn = Conexion.Conect;

                // Ejecutar una consulta SQL para insertar la nueva tarea en la tabla 'tasks'
                using var cmd = new NpgsqlCommand("INSERT INTO tasks (title, description, status, due_date, user_id, create_date) VALUES (@title, @description, @status, @dueDate, @userId, @createDate)", conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@dueDate", dueDate);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@createDate", DateTime.Now);

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

                // Conexion a la base de datos PostgreSQL
                using var conn = Conexion.Conect;

                // Ejecutar una consulta SQL para actualizar los datos de la tarea en la tabla 'tasks' según el 'id_task' proporcionado
                using var cmd = new NpgsqlCommand("UPDATE tasks SET title = @title, description = @description, status = @status, due_date = @dueDate WHERE id_task = @idTask", conn);
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
                // Conexion a la base de datos PostgreSQL
                using var conn = Conexion.Conect;

                // Ejecutar una consulta SQL para eliminar la tarea correspondiente de la tabla 'tasks' según el 'id_task' proporcionado
                using var cmd = new NpgsqlCommand("DELETE FROM tasks WHERE id_task = @idTask", conn);
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
                // Conexion a la base de datos PostgreSQL
                //using var conn = Conexion.Conect;
                NpgsqlConnection connection = Conexion.Conect();

                // Ejecutar una consulta SQL para buscar la tarea correspondiente en la tabla 'tasks' según el 'id' proporcionado
                using var cmd = new NpgsqlCommand("SELECT * FROM tasks WHERE id_task = @idTask", connection);
                cmd.Parameters.AddWithValue("@idTask", id);

                // Ejecutar la consulta y obtener el resultado
                using var reader = await cmd.ExecuteReaderAsync();

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
