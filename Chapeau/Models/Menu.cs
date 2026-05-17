namespace Chapeau.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }

        public Menu() { }
        public Menu(int menuId, string menuName)
        {
            MenuId = menuId;
            MenuName = menuName;
        }
    }
}
