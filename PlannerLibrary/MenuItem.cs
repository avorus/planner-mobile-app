namespace PlannerLibrary
{
    /// <summary>
    /// Класс элемента боковой панели меню
    /// </summary>
    public class MenuItem
    {
        public string Name { get; set; } //имя
        public string Icon { get; set; } //картинка
        public MenuItem(string icon, string name)
        {
            Icon = icon;
            Name = name;
        }
    }
}
