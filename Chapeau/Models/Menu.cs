using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public Category Category { get; set; }
        public Card Card { get; set; }
        public List<MenuItem> MenuItems { get; set; }

        public Menu() { }
        public Menu(int menuId, Card card, Category category)
        {
            MenuId = menuId;
            Card = card;
            Category = category;
        }
    }
}
