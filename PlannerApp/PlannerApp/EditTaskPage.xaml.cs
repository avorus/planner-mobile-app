using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PlannerLibrary;

namespace PlannerApp
{
    /// <summary>
    /// Страница редактирования задачи
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTaskPage : ContentPage
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
         * SaveButton - кнопка для редактирования задачи
         */

        long oneDay = 864 * (long)Math.Pow(10, 9); //одни сутки в Ticks

        public Task EditedTask { get; set; } //старый экземпляр задачи 

        public Task NewTask { get; set; } //новый экземпляр задачи

        public EditTaskPage(Task editedTask)
        {
            NavigationPage.SetHasBackButton(this, false); //отключение навигации с помощью кнопки "Назад"
            EditedTask = editedTask;
            InitializeComponent();
            MainDescriptionEntry.Text = EditedTask.MainDescription;
            DetailDescriptionEditor.Text = EditedTask.DetailDescription;
            DeadlineSwitch.IsToggled = EditedTask.IsDeadLine;
            StartDatePicker.Date = new DateTime(EditedTask.BeginDate.Ticks - (EditedTask.BeginDate.Ticks % oneDay));
            StartTimePicker.Time = new TimeSpan(EditedTask.BeginDate.Ticks % oneDay);
            EndDatePicker.Date = new DateTime(EditedTask.EndDate.Ticks - (EditedTask.EndDate.Ticks % oneDay));
            EndTimePicker.Time = new TimeSpan(EditedTask.EndDate.Ticks % oneDay);
            StartDatePicker.Date = new DateTime(EditedTask.BeginDate.Ticks - (EditedTask.BeginDate.Ticks % oneDay));
            StartTimePicker.Time = new TimeSpan(EditedTask.BeginDate.Ticks % oneDay);
            EndDatePicker.Date = new DateTime(EditedTask.EndDate.Ticks - (EditedTask.EndDate.Ticks % oneDay));
            EndTimePicker.Time = new TimeSpan(EditedTask.EndDate.Ticks % oneDay);
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
        /// Деактивирует кнопку для редактирования задачи из-за ввода некорректных данных.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void CheckInput(object sender, EventArgs e)
        {
            SaveButton.IsEnabled = (DeadlineSwitch.IsToggled || (StartDatePicker.Date < EndDatePicker.Date || StartDatePicker.Date == EndDatePicker.Date && StartTimePicker.Time <= EndTimePicker.Time))
                && MainDescriptionEntry.Text != string.Empty && DetailDescriptionEditor.Text != string.Empty;
            SaveButton.BackgroundColor = SaveButton.IsEnabled ? Color.FromHex("#ffc40b") : Color.FromHex("#555555");
        }

        /// <summary>
        /// Обработка события нажатия на кнопку для редактирования задачи.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void OnSaveButtonCliked(object sender, EventArgs e)
        {
            if (MainDescriptionEntry.Text != string.Empty && DetailDescriptionEditor.Text != string.Empty)
            {
                if (!DeadlineSwitch.IsToggled)
                {
                    NewTask = new Task(
                        MainDescriptionEntry.Text,
                        DetailDescriptionEditor.Text,
                        new DateTime(StartDatePicker.Date.Ticks + StartTimePicker.Time.Ticks),
                        new DateTime(EndDatePicker.Date.Ticks + EndTimePicker.Time.Ticks));
                    User.Tasks.Remove(EditedTask);
                    User.Tasks.Add(NewTask);
                }
                else
                {
                    NewTask = new Task(
                    MainDescriptionEntry.Text,
                    DetailDescriptionEditor.Text,
                    new DateTime(StartDatePicker.Date.Ticks + StartTimePicker.Time.Ticks));
                    User.Tasks.Remove(EditedTask);
                    User.Tasks.Add(NewTask);
                }
                EditedTask = NewTask;
                CheckInput(this, new EventArgs());
                DisplayAlert("Теперь всё точно!", "Изменения сохранены", "OK");
            }
        }

        protected override bool OnBackButtonPressed() { return true; }
    }
}