using System.Collections.Generic;

namespace PlannerLibrary
{
    /// <summary>
    /// Класс пользователя
    /// </summary>
    public static class User
    {
        public static List<Task> Tasks { get; set; } //список текущих задач
        public static List<Task> DeletedTasks { get; set; } //список удалённых задач

        /// <summary>
        /// Сериализует список текущих и удалённых задач
        /// </summary>
        public static void Serialize()
        {
            XML.Serialize(Tasks, "tasks.xml");
            XML.Serialize(DeletedTasks, "deletedtasks.xml");
        }

        /// <summary>
        /// Десериализует список текущих и удалённых задач
        /// </summary>
        public static void Deserialize()
        {
            Tasks = XML.Deserialize("tasks.xml");
            DeletedTasks = XML.Deserialize("deletedtasks.xml");
        }
    }
}
