using System;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PlannerLibrary;

namespace PlannerApp
{
    /// <summary>
    /// Страница удаления задачи
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeletedTasksPage : ContentPage
    {
        public ObservableCollection<Task> DeletedTasksList { get; set; } //список удалённых задач

        public DeletedTasksPage()
        {
            InitializeComponent();
            DeletedTasksList = new ObservableCollection<Task>(User.DeletedTasks.OrderBy(task => task.BeginDate));
            BindingContext = this;
            CheckInput();
        }

        /// <summary>
        /// Обработка события нажатия на задачу для восстановления.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private async void DeletedTaskClick(object sender, EventArgs e)
        {
            //Получение соответствующей задачи
            Task task = ((ItemTappedEventArgs)e).Item as Task;

            //Отображение всплывающего окна с запросом разрешения на восстановление задачи
            bool accept = await DisplayAlert("Восстановить задачу?", "Задача будет восстановлена.", "ОК", "Отмена");
            if (accept)
            {
                //Удаление задачи из списка удалённых задач
                DeletedTasksList.Remove(task);
                User.DeletedTasks = DeletedTasksList.ToList();
                //Добавление задачи в список текущих задач
                User.Tasks.Add(task);
            }
            CheckInput();
        }

        /// <summary>
        /// Деактивирует кнопку для очистки списка удалённых задачи в связи с пустотой списка.
        /// </summary>
        private void CheckInput()
        {
            DeleteButton.IsEnabled = DeletedTasksList.Count != 0;
            DeleteButton.BackgroundColor = DeleteButton.IsEnabled ? Color.FromHex("#ffc40b") : Color.FromHex("#555555");
        }

        /// <summary>
        /// Обработка события нажатия на кнопку для очистки списка удалённых задач.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private async void OnDeleteButtonCliked(object sender, EventArgs e)
        {
            //Отображение всплывающего окна с запросом разрешения на очистку списка удалённых задач.
            bool accept = await DisplayAlert("Очистить список задач?", "Задачи будут удалены навсегда.", "ОК", "Отмена");
            if (accept)
            {
                DeletedTasksList.Clear();
                User.DeletedTasks = DeletedTasksList.ToList();
            }
            CheckInput();
        }
    }
}