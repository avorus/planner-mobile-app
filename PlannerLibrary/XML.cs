using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace PlannerLibrary
{
    /// <summary>
    /// Класс работы с файлом XML-формата
    /// </summary>
    public static class XML
    {
        /// <summary>
        /// Возвращает путь к файлу с указанным именем на устройстве.
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>полный путь к файлу на устройстве</returns>
        static string GetPath(string fileName)
        {
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(DocumentsPath, fileName);
        }

        /// <summary>
        /// Сериализует данные типа List<Task> в файл с указанным именем.
        /// </summary>
        /// <param name="task">данные для сериализации</param>
        /// <param name="fileName">имя файла</param>
        public static void Serialize(List<Task> task, string fileName)
        {
            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(List<Task>));
            using (XmlWriter xmlWriter = XmlWriter.Create(GetPath(fileName), new XmlWriterSettings() { Indent = true }))
            {
                dataContractSerializer.WriteObject(xmlWriter, task);
            }
        }

        /// <summary>
        /// Десериализует данные типа List<Task> из файла с указанным именем.
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>десериализованные данные</returns>
        public static List<Task> Deserialize(string fileName)
        {
            try
            {
                List<Task> tasks;
                using (StreamReader streamReader = new StreamReader(GetPath(fileName)))
                {
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(List<Task>));
                    using (XmlReader xmlReader = XmlReader.Create(GetPath(fileName)))
                    {
                        tasks = dataContractSerializer.ReadObject(xmlReader) as List<Task>;
                    }
                }
                return tasks;
            }
            catch
            {
                return new List<Task>();
            }
        }
    }
}
