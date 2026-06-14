using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public Category Category { get; set; }
        public Card Card { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public Menu() { }
       
    }
}
