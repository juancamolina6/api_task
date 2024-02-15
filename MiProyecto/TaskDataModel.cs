namespace MiProyecto.Models
{
    public class TaskDataModel
    {
        // Propiedades de la tarea
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        // Otras propiedades según sea necesario
    }
}
