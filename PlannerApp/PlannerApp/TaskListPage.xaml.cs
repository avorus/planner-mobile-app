using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PlannerLibrary;

namespace PlannerApp
{
    /// <summary>
    /// Страница списка текущих задач
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskListPage : ContentPage
    {
        public ObservableCollection<Task> TasksList { get; set; } //список текущих задач
        public ObservableCollection<string> Categories { get; set; } //список текущих категорий

        bool IsCheckBoxEventExecuted; //активация или деактивация события CheckBox
        bool IsPickerEventExecuted; //активация или деактивация события Picker

        public TaskListPage()
        {
            InitializeComponent();
            TasksList = new ObservableCollection<Task>(User.Tasks.OrderBy(task => task.BeginDate));
            CreateCategories();
            BindingContext = this;
            IsCheckBoxEventExecuted = true;
        }

        /// <summary>
        /// Обработка события прокрутки влево или вправо.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private async void OnSwipeEnded(object sender, EventArgs e)
        {
            //Удаление задачи для прокрутки вправо
            if ((e as SwipeEndedEventArgs).SwipeDirection == SwipeDirection.Right)
            {
                bool accept = await DisplayAlert("Удалить задачу?", "Задача будет удалена из Вашего списка.", "ОК", "Отмена");
                if (accept)
                {
                    IsCheckBoxEventExecuted = false;
                    Task task = ((sender as SwipeView).Parent as ViewCell).BindingContext as Task;
                    TasksList.Remove(task);
                    User.Tasks.Remove(task);
                    User.DeletedTasks.Add(task);
                    RefreshCategories();

                }
            }
            //Редактирование задачи для прокрутки влево
            else if ((e as SwipeEndedEventArgs).SwipeDirection == SwipeDirection.Left)
            {
                if ((((sender as SwipeView).Parent as ViewCell).BindingContext as Task).IsCompleted)
                {
                    await DisplayAlert("Вы уже выполнили задачу", "Нельзя изменить завершенную задачу", "OK");
                }
                else
                {
                    await Navigation.PushAsync(new EditTaskPage(((sender as SwipeView).Parent as ViewCell).BindingContext as Task));
                }
            }
            (sender as SwipeView).Close();
            IsCheckBoxEventExecuted = true;
        }

        /// <summary>
        /// Обработка события переключения CheckBox для изменения статуса задачи в связи с её выполнением.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void OnIsCompleteCheckBoxChanged(object sender, EventArgs e)
        {
            if (IsCheckBoxEventExecuted)
            {
                Task editedTask = ((sender as CheckBox).Parent.Parent.Parent as ViewCell).BindingContext as Task;
                (((sender as CheckBox).Parent.Parent.Parent as ViewCell).BindingContext as Task).IsCompleted = (sender as CheckBox).IsChecked;
                User.Tasks[User.Tasks.IndexOf(editedTask)].IsCompleted = (sender as CheckBox).IsChecked;
                RefreshCategories();
            }
        }

        /// <summary>
        /// Обработка события выбора элемента в Picker для фильтрации списка текущих задач.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnCategoryChanged(object sender, EventArgs e)
        {
            if (IsPickerEventExecuted)
            {
                IsCheckBoxEventExecuted = false;
                IsPickerEventExecuted = false;
                TasksList.Clear();
                List<Task> newTasks = new List<Task>();
                switch (CategoryPicker.SelectedIndex)
                { 
                    //Дефолтное значение
                    case -1:
                        newTasks = User.Tasks.OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Все
                    case 0:
                        newTasks = User.Tasks.OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Дедлайны
                    case 1:
                        newTasks = User.Tasks.Where(task => task.IsDeadLine).OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Выполненные
                    case 2:
                        newTasks = User.Tasks.Where(task => task.IsCompleted).OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Невыполненные
                    case 3:
                        newTasks = User.Tasks.Where(task => !task.IsCompleted).OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Просроченные
                    case 4:
                        newTasks = User.Tasks.Where(task => task.EndDate < DateTime.Now && !task.IsCompleted).OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Вчера
                    case 5:
                        DateTime yesterday = new DateTime(DateTime.Now.Ticks - Utils.oneDay);
                        newTasks = User.Tasks.
                            Where(task => Utils.Date(task.BeginDate) <= Utils.Date(yesterday) && Utils.Date(yesterday) <= Utils.Date(task.EndDate)
                            || (task.IsDeadLine && Utils.Date(new DateTime(yesterday.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).
                            OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Сегодня
                    case 6:
                        DateTime now = DateTime.Now;
                        newTasks = User.Tasks.
                            Where(task => Utils.Date(task.BeginDate) <= Utils.Date(now) && Utils.Date(now) <= Utils.Date(task.EndDate)
                            || (task.IsDeadLine && Utils.Date(new DateTime(now.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).
                            OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                    //Завтра
                    case 7:
                        DateTime tomorrow = new DateTime(DateTime.Now.Ticks + Utils.oneDay);
                        newTasks = User.Tasks.
                            Where(task => Utils.Date(task.BeginDate) <= Utils.Date(tomorrow) && Utils.Date(tomorrow) <= Utils.Date(task.EndDate)
                            || (task.IsDeadLine && Utils.Date(new DateTime(tomorrow.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).
                            OrderBy(task => task.BeginDate).Select(task => task).ToList();
                        break;
                }
                newTasks.ForEach(task => TasksList.Add(task));
                IsCheckBoxEventExecuted = true;
                IsPickerEventExecuted = true;
            }
        }

        /// <summary>
        /// Инициализирует список текущих категорий.
        /// </summary>
        private void CreateCategories()
        {
            IsPickerEventExecuted = false;
            DateTime yesterday = new DateTime(DateTime.Now.Ticks - Utils.oneDay);
            DateTime now = DateTime.Now;
            DateTime tomorrow = new DateTime(DateTime.Now.Ticks + Utils.oneDay);
            Categories = new ObservableCollection<string>
            {
                $"Все ({User.Tasks.Count})",
                $"Дедлайны ({User.Tasks.Where(task => task.IsDeadLine).Count()})",
                $"Выполненные ({User.Tasks.Where(task => task.IsCompleted).Count()})",
                $"Невыполненные ({User.Tasks.Where(task => !task.IsCompleted).Count()})",
                $"Просроченные ({User.Tasks.Where(task => task.EndDate < DateTime.Now && !task.IsCompleted).Count()})",
                $"Вчера ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(yesterday) && Utils.Date(yesterday) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(yesterday.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})",
                $"Сегодня ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(now) && Utils.Date(now) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(now.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})",
                $"Завтра ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(tomorrow) && Utils.Date(tomorrow) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(tomorrow.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})"
            };
            IsPickerEventExecuted = true;
        }

        /// <summary>
        /// Обновляет список текущих категорий.
        /// </summary>
        private void RefreshCategories()
        {
            IsPickerEventExecuted = false;
            DateTime yesterday = new DateTime(DateTime.Now.Ticks - Utils.oneDay);
            DateTime now = DateTime.Now;
            DateTime tomorrow = new DateTime(DateTime.Now.Ticks + Utils.oneDay);
            Categories[0] = $"Все ({User.Tasks.Count})";
            Categories[1] = $"Дедлайны ({User.Tasks.Where(task => task.IsDeadLine).Count()})";
            Categories[2] = $"Выполненные ({User.Tasks.Where(task => task.IsCompleted).Count()})";
            Categories[3] = $"Невыполненные ({User.Tasks.Where(task => !task.IsCompleted).Count()})";
            Categories[4] = $"Просроченные ({User.Tasks.Where(task => task.EndDate < DateTime.Now && !task.IsCompleted).Count()})";
            Categories[5] = $"Вчера ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(yesterday) && Utils.Date(yesterday) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(yesterday.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})";
            Categories[6] = $"Сегодня ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(now) && Utils.Date(now) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(now.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})";
            Categories[7] = $"Завтра ({User.Tasks.Where(task => Utils.Date(task.BeginDate) <= Utils.Date(tomorrow) && Utils.Date(tomorrow) <= Utils.Date(task.EndDate) || (task.IsDeadLine && Utils.Date(new DateTime(tomorrow.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).Count()})";
            IsPickerEventExecuted = true;
        }
    }
}