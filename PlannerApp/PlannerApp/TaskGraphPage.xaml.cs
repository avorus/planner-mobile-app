using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PlannerLibrary;

namespace PlannerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskGraphPage : ContentPage
    {
        List<Task> Tasks { get; set; }

        public TaskGraphPage()
        {
            Tasks = User.Tasks;
            InitializeComponent();
            CurrentDatePicker.Date = DateTime.Now;
            LoadElements(CurrentDatePicker.Date);
        }

        private void OnCurrentDateSelected(object sender, DateChangedEventArgs e)
        {
            LoadElements(e.NewDate);
        }

        void LoadElements(DateTime dateList)
        {
            DetailStackLayout.Children.Clear();

            List<Task> thisDayTasks = Tasks.
                Where(task => 
                Utils.Date(task.BeginDate) <= Utils.Date(dateList) && Utils.Date(dateList) <= Utils.Date(task.EndDate) 
                || (task.IsDeadLine && Utils.Date(new DateTime(dateList.Ticks + Utils.oneDay)).ToString("dd.MM HH:mm") == task.EndDateText)).
                ToList();//список задач на заданный день

            Dictionary<string, List<Task>> panel = new Dictionary<string, List<Task>>();

            for (int i = 0; i < 48; ++i)
            {
                string time = i % 2 == 0 ? $"{(i / 2):d2}:00" : $"{(i / 2):d2}:30";
                List<Task> thisPeriodTasks = thisDayTasks.
                    Where(task => Utils.IsTaskInPeriod(task, i, dateList)).
                    OrderByDescending(task => (Utils.Date(task.EndDate) == dateList ? Utils.Time(task.EndDate) : new TimeSpan(Utils.oneDay - Utils.oneMinute)) - (Utils.Date(task.BeginDate) == dateList ? Utils.Time(task.BeginDate) : Utils.Time(Utils.Date(task.BeginDate)))).
                    ToList();
                panel.Add(time, thisPeriodTasks);
            }

            for (int i = 0; i < 48; ++i)
            {
                StackLayout detailStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

                string key = i % 2 == 0 ? $"{(i / 2):d2}:00" : $"{(i / 2):d2}:30";

                //Добавление текста со временем
                detailStackLayout.Children.Add(
                    new Label
                    {
                        Text = key,
                        Margin = new Thickness(5, 0, 5, 0),
                        VerticalTextAlignment = TextAlignment.Start,
                        TextColor = Color.FromHex("#f7fbf7"),
                        FontSize = 16,
                        HeightRequest = 40
                    });

                //Добавление цветных блоков задач
                int count = 0;
                foreach (var task in panel[key])
                {
                    ++count;
                    if (count <= 3)
                    {
                        detailStackLayout.Children.Add(
                            new Label
                            {
                                Text = " ",
                                Margin = new Thickness(5, 0, 5, 0),
                                BackgroundColor = task.IsCompleted ? Color.FromHex("#0ed145") : (task.IsDeadLine ? Color.FromHex("#ff4040") : Color.FromHex("#ffc40b")),
                                FontSize = 16,
                                HeightRequest = 40,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            });
                    }
                    else
                    {
                        break;
                    }
                }

                //Добавление недостающих блоков для выравнивания
                for (int j = 0; j < 3 - panel[key].Count; ++j)
                {
                    detailStackLayout.Children.Add(
                        new Label
                        {
                            Text = " ",
                            Margin = new Thickness(5, 0, 5, 0),
                            BackgroundColor = Color.FromHex("#131313"),
                            FontSize = 16,
                            HeightRequest = 40,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        });
                }

                Button detailButton = new Button
                {
                    Text = "...",
                    Margin = new Thickness(5, 0, 5, 0),
                    TextColor = Color.FromHex("#f7fbf7"),
                    FontSize = 13,
                    HeightRequest = 40,
                    BackgroundColor = Color.FromHex("#131313"),
                    IsVisible = panel[key].Count != 0,
                    IsEnabled = panel[key].Count != 0
                };

                string period = $"{dateList.ToString("dd.MM.yyyy")} {TimePeriod(i)}";

                detailButton.Clicked += (sender, e) =>
                {
                    List<string> tasksList = new List<string>();
                    foreach (var task in panel[key])
                    {
                        tasksList.Add($"{task.MainDescription} {(task.IsDeadLine ? "(дедлайн)" : "")} {(task.IsCompleted ? "- выполнено" : "")}" +
                            $"\n{(task.IsDeadLine ? task.EndDateText : $"{task.BeginDateText}-{task.EndDateText}")}\n");
                    }
                    DisplayAlert($"Ваши задачи на {period}", string.Join("\n", tasksList), "OK");
                };

                //Добавление кнопки Подробнее
                detailStackLayout.Children.Add(detailButton);

                //Добавление дочернего в главный
                DetailStackLayout.Children.Add(detailStackLayout);
            }

            StackLayout endDetailStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            endDetailStackLayout.Children.Add(
                new Label
                {
                    Text = "00:00",
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalTextAlignment = TextAlignment.Start,
                    TextColor = Color.FromHex("#f7fbf7"),
                    FontSize = 16,
                    HeightRequest = 40
                });
            for (int j = 0; j < 3; ++j)
            {
                endDetailStackLayout.Children.Add(
                    new Label
                    {
                        Text = " ",
                        Margin = new Thickness(5, 0, 5, 0),
                        BackgroundColor = Color.FromHex("#131313"),
                        FontSize = 16,
                        HeightRequest = 40,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    });
            }
            DetailStackLayout.Children.Add(endDetailStackLayout);
        }

        static string TimePeriod(int index)
        {
            //return $"{(index / 2):d2}:{(index % 2 == 0 ? "00" : "30")} - {(index / 2):d2}:{(index % 2 == 0 ? "29" : "59")}";
            return $"{(index % 2 == 0 ? $"{(index / 2):d2}:00 - {(index / 2):d2}:30" : $"{(index / 2):d2}:30 - {((index + 1) / 2):d2}:00")}";
        }
    }
}