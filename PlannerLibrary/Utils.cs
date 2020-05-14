using System;

namespace PlannerLibrary
{
    /// <summary>
    /// Класс вспомогательных средств
    /// </summary>
    public static class Utils
    {
        public static long oneMinute = (long)(6 * Math.Pow(10, 8)); //одна минута в Ticks
        public static long oneHour = 60 * oneMinute; //один час в Ticks
        public static long oneDay = 24 * oneHour; //один день в Ticks
        public static long oneMonth = 30 * oneDay; //один месяц в Ticks

        /// <summary>
        /// Возвращает дату входного параметра без времени.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime Date(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks - dateTime.Ticks % oneDay);
        }

        /// <summary>
        /// Возвращает время входного параметра без даты.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan Time(DateTime dateTime)
        {
            return new TimeSpan(dateTime.Ticks % oneDay);
        }

        /// <summary>
        /// Определяет, входит ли задача в период с указанным индексом
        /// </summary>
        /// <param name="task">задача</param>
        /// <param name="index">индекс периода времени</param>
        /// <param name="currentDate">текущая дата для периода времени</param>
        /// <returns></returns>
        public static bool IsTaskInPeriod(Task task, int index, DateTime currentDate)
        {
            TimeSpan startTime = new TimeSpan(index % 2 == 0 ? (index / 2) * oneHour : (index / 2) * oneHour + 30 * oneMinute); //время начала периода
            TimeSpan endTime = new TimeSpan(index % 2 == 0 ? (index / 2) * oneHour + 29 * oneMinute : (index / 2) * oneHour + 59 * oneMinute); //время окончания периода
            if (task.IsDeadLine || task.BeginDate == task.EndDate)
            {
                if (index != 47)
                {
                    return startTime < Time(task.EndDate) && Time(task.EndDate) <= Time(new DateTime(endTime.Ticks + oneMinute));
                }
                else
                {
                    string nextDay = Date(new DateTime(currentDate.Ticks + oneDay)).ToString("dd.MM HH:mm");
                    return startTime < Time(task.EndDate) && (Time(task.EndDate) <= endTime) || nextDay == task.EndDateText;
                }
            }
            else
            {
                DateTime startPeriod = new DateTime(Date(currentDate).Ticks + startTime.Ticks);
                DateTime endPeriod = new DateTime(Date(currentDate).Ticks + endTime.Ticks);
                return !(task.EndDate <= startPeriod || endPeriod < task.BeginDate);
            }
        }
    }
}
