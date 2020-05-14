using System;
using System.Runtime.Serialization;

namespace PlannerLibrary
{
    /// <summary>
    /// Класс задачи
    /// </summary>
    [DataContract]
    public class Task
    {
        [DataMember]
        public string MainDescription { get; set; } //название
        [DataMember]
        public string DetailDescription { get; set; } //описание
        [DataMember]
        public DateTime BeginDate { get; set; } //дата и время начала
        [DataMember]
        public string BeginDateText { get; set; } //строковое представление начала
        [DataMember]
        public DateTime EndDate { get; set; } //дата и время окончания
        [DataMember]
        public string EndDateText { get; set; } //строковое представление окончания
        [DataMember]
        public bool IsCompleted { get; set; } //параметр, выполнена ли задача
        [DataMember]
        public bool IsDeadLine { get; set; } //параметр, является ли задача дедлайном

        public Task() { }

        /// <summary>
        /// Конструктор для создания задачи, являющейся дедлайном.
        /// </summary>
        /// <param name="mainDescription">название</param>
        /// <param name="detailDescription">описание</param>
        /// <param name="endDate">дата и время срока</param>
        public Task(string mainDescription, string detailDescription, DateTime endDate) 
        {
            MainDescription = mainDescription;
            DetailDescription = detailDescription;
            BeginDate = endDate;
            BeginDateText = "";
            EndDate = endDate;
            EndDateText = endDate.ToString("dd.MM HH:mm");
            IsCompleted = false;
            IsDeadLine = true; 
        }

        /// <summary>
        /// Конструктор для создания задачи, не являющейся дедлайном.
        /// </summary>
        /// <param name="mainDescription">название</param>
        /// <param name="detailDescription">описание</param>
        /// <param name="beginDate">дата и время начала</param>
        /// <param name="endDate">дата и время окончания</param>
        public Task(string mainDescription, string detailDescription, DateTime beginDate, DateTime endDate)
        {
            MainDescription = mainDescription;
            DetailDescription = detailDescription;
            BeginDate = beginDate;
            BeginDateText = beginDate.ToString("dd.MM HH:mm");
            EndDate = endDate;
            EndDateText = endDate.ToString("dd.MM HH:mm");
            IsCompleted = false;
            IsDeadLine = false;
        }
    }
}
