using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PlannerLibrary;

namespace PlannerApp
{
    /// <summary>
    /// Страница добавления задачи
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewTaskPage : ContentPage
    {
        /*
         * Об элементах управления:
         * MainDescriptionEntry - поле для ввода названия задачи
         * DetailDescriptionEditor - поле для ввода описания задачи
         * DeadlineSwitch - переключатель дедлайна
         * StartDatePicker - выбор даты начала
         * StartTimePicker - выбор времени начала
         * EndDatePicker - выбор даты окончания 
         * EndTimePicker - выбор времени окончания
         * AddButton - кнопка для добавления задачи
         */

        public AddNewTaskPage()
        {
            InitializeComponent();
            MainDescriptionEntry.Text = string.Empty;
            DetailDescriptionEditor.Text = string.Empty;
            CheckInput(this, new EventArgs());
        }

        /// <summary>
        /// Обработка события переключателя дедлайна.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void OnDeadlineSwitchToggled(object sender, EventArgs e)
        {
            if (DeadlineSwitch.IsToggled)
            {
                StartLabel.Text = "Срок:";
                EndLabel.IsVisible = false;
                EndDatePicker.IsVisible = false;
                EndDatePicker.Date = StartDatePicker.Date;
                EndDatePicker.IsEnabled = false;
                EndTimePicker.IsVisible = false;
                EndTimePicker.Time = StartTimePicker.Time;
                EndTimePicker.IsEnabled = false;
            }
            else
            {
                StartLabel.Text = "Начало:";
                EndLabel.IsVisible = true;
                EndDatePicker.IsVisible = true;
                EndDatePicker.IsEnabled = true;
                EndTimePicker.IsVisible = true;
                EndTimePicker.IsEnabled = true;
            }
            CheckInput(this, new EventArgs());
        }

        /// <summary>
        /// Деактивирует кнопку для добавления задачи из-за ввода некорректных данных.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void CheckInput(object sender, EventArgs e)
        {
            AddButton.IsEnabled = (DeadlineSwitch.IsToggled || (StartDatePicker.Date < EndDatePicker.Date || StartDatePicker.Date == EndDatePicker.Date && StartTimePicker.Time <= EndTimePicker.Time))
                && MainDescriptionEntry.Text != string.Empty && DetailDescriptionEditor.Text != string.Empty;
            AddButton.BackgroundColor = AddButton.IsEnabled ? Color.FromHex("#ffc40b") : Color.FromHex("#555555");
        }

        /// <summary>
        /// Обработка события нажатия на кнопку для добавления задачи.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void OnAddButtonCliked(object sender, EventArgs e)
        {
            //Создание нового экземпляра задачи
            Task newTask;
            if (!DeadlineSwitch.IsToggled) //если задача не является дедлайном
            {
                newTask = 
                    new Task(
                    MainDescriptionEntry.Text,
                    DetailDescriptionEditor.Text,
                    new DateTime(StartDatePicker.Date.Ticks + StartTimePicker.Time.Ticks),
                    new DateTime(EndDatePicker.Date.Ticks + EndTimePicker.Time.Ticks));
            }
            else
            {
                newTask = 
                new Task(
                MainDescriptionEntry.Text,
                DetailDescriptionEditor.Text,
                new DateTime(StartDatePicker.Date.Ticks + StartTimePicker.Time.Ticks));
            }

            //Добавление нового экземпляра задачи в список текущих задач пользователя
            User.Tasks.Add(newTask);
            DisplayAlert("Теперь вы точно ничего не забудете!", "Задача добавлена в ваш список", "OK");

            //Сброс ввода до значений по умолчанию
            MainDescriptionEntry.Text = string.Empty;
            DetailDescriptionEditor.Text = string.Empty;
            DeadlineSwitch.IsToggled = false;
            StartDatePicker.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimePicker.Time = new TimeSpan(12, 0, 0);
            EndDatePicker.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            EndTimePicker.Time = new TimeSpan(12, 0, 0);
            StartDatePicker.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimePicker.Time = new TimeSpan(12, 0, 0);
            EndDatePicker.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            EndTimePicker.Time = new TimeSpan(12, 0, 0);
            CheckInput(this, new EventArgs());
        }
    }
}