using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PlannerLibrary;

namespace PlannerApp
{
    /// <summary>
    /// Главная страница с выдвижной боковой панелью меню
    /// </summary>
    public partial class MainPage : MasterDetailPage
    {
        public List<PlannerLibrary.MenuItem> MenuItems { get; set; } //элементы меню

        public MainPage()
        {
            //Кнопка для отображения страницы добавления задачи
            ToolbarItem addTask = new ToolbarItem()
            {
                IconImageSource = "AddNewTaskImage.png",
                Text = "addTask"
            };
            //Обработка нажатия на кнопку для отображения страницы добавления задачи
            addTask.Clicked += (sender, e) =>
            {
                Detail = new NavigationPage(new AddNewTaskPage());
            };
            ToolbarItems.Add(addTask);

            //Кнопка для отображения страницы списка текущих задач
            ToolbarItem taskList = new ToolbarItem()
            {
                IconImageSource = "TaskListImage.png",
                Text = "taskList"
            };
            //Обработка нажатия на кнопку для отображения страницы списка текущих задач
            taskList.Clicked += (sender, e) =>
            {
                Detail = new NavigationPage(new TaskListPage());
            };
            ToolbarItems.Add(taskList);

            //Десериализация задач пользователя
            User.Deserialize();

            InitializeComponent();
            MenuItems = new List<PlannerLibrary.MenuItem>()
            {
                new PlannerLibrary.MenuItem("TaskGraphImage.png","График задач"),
                new PlannerLibrary.MenuItem("DeletedTasksImage.png","Удаленные задачи"),
                new PlannerLibrary.MenuItem("HelpImage.png","Помощь")
            };
            BindingContext = this;
            Detail = new NavigationPage(new TaskListPage());
        }

        /// <summary>
        /// Обработка события нажатия на элемент на боковой панели меню.
        /// </summary>
        /// <param name="sender">источник вызова</param>
        /// <param name="e">дополнительные параметры</param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            IsPresented = false;
            string name = ((PlannerLibrary.MenuItem)((ItemTappedEventArgs)e).Item).Name;
            MenuList.SelectedItem = null;
            switch (name)
            {
                case "График задач":
                    Detail = new NavigationPage(new TaskGraphPage());
                    break;
                case "Удаленные задачи":
                    Detail = new NavigationPage(new DeletedTasksPage());
                    break;
                case "Помощь":
                    Detail = new NavigationPage(new HelpPage());
                    break;
            }
        }
    }
}
